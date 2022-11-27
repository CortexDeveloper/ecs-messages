using System.Collections.Generic;
using System.Linq;
using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessagesStatsWindow : EditorWindow
    {
        private int _selectedTab;
        private int _logsEnabled;
        
        private MessageLifetime _messageLifetimeFilter;

        private List<string> _worldsList = new();
        private int _selectedWorld;
        private World SelectedWorld => World.All.GetWorldWithName(_worldsList[_selectedWorld]);
        
        private static EndSimulationEntityCommandBufferSystem GetEcbSystemInWorld(World world) => 
            world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        [MenuItem("DOTS/ECSMessages/Stats")]
        public static void Init()
        {
            MessagesStatsWindow statsWindow = (MessagesStatsWindow)GetWindow(
                typeof(MessagesStatsWindow), 
                false, 
                "Message Broadcaster");
            
            statsWindow.Show();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
            
            DrawWorldPopup();
            DrawStats();
            Repaint();
        }
        
        private void DrawWorldPopup()
        {
            _worldsList.Clear();

            for (int i = 0; i < World.All.Count; i++) 
                _worldsList.Add(default);

            int j = 0;
            foreach (World world in World.All)
            {
                if (j < World.All.Count) 
                    _worldsList[j] = world.Name;
                else
                    break;

                j++;
            }

            _worldsList = _worldsList.Where(w => !w.Contains("LoadingWorld")).ToList();
            _selectedWorld = EditorGUILayout.Popup("World", _selectedWorld, _worldsList.ToArray());
        }

        private void DrawStats()
        {
            DrawMessagesStats();
            EditorGUILayout.Space(25f);
            DrawRemoveAPI();
        }

        private void DrawMessagesStats()
        {
            string worldName = SelectedWorld.Name;
                
            EditorGUILayout.LabelField($"Messages:          {MessagesStats.StatsMap[worldName].ActiveMessagesCount}");
            EditorGUILayout.LabelField($"Attached Messages: {MessagesStats.StatsMap[worldName].ActiveAttachedMessagesCount}");
            EditorGUILayout.LabelField($"Events:            {MessagesStats.StatsMap[worldName].ActiveEventsCount}");
            EditorGUILayout.LabelField($"Commands:          {MessagesStats.StatsMap[worldName].ActiveCommandsCount}");
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField($"OneFrame:          {MessagesStats.StatsMap[worldName].ActiveOneFrameMessagesCount}");
            EditorGUILayout.LabelField($"TimeRange:         {MessagesStats.StatsMap[worldName].ActiveTimeRangeMessagesCount}");
            EditorGUILayout.LabelField($"Unlimited:         {MessagesStats.StatsMap[worldName].ActiveUnlimitedLifetimeMessagesCount}");
        }
        
        private void DrawRemoveAPI()
        {
            _messageLifetimeFilter = (MessageLifetime)EditorGUILayout.EnumPopup("Lifetime Filter: ", _messageLifetimeFilter);

            if (GUILayout.Button("Remove Messages by Lifetime Filter"))
                MessageBroadcaster.RemoveCommonWithLifetime(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), _messageLifetimeFilter);

            if (GUILayout.Button("Remove All")) 
                MessageBroadcaster.RemoveAllMessages(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer());
        }
    }
}