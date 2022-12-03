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
        private bool _statsEnabled;
        
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
            _statsEnabled = EditorGUILayout.Toggle("Stats Enabled: ", _statsEnabled);
            MessagesStats.Enabled = _statsEnabled;
            
            EditorGUILayout.Space(10f);
            DrawMessagesStats();
            EditorGUILayout.Space(25f);
            DrawRemoveAPI();
        }

        private void DrawMessagesStats()
        {
            string worldName = SelectedWorld.Name;
            
            if (!MessagesStats.StatsMap.ContainsKey(worldName))
                return;
                
            EditorGUILayout.LabelField($"Messages: {MessagesStats.StatsMap[worldName].ActiveMessagesCount}");
            EditorGUILayout.LabelField($"Attached Messages: {MessagesStats.StatsMap[worldName].ActiveAttachedMessagesCount}");
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField($"OneFrame: {MessagesStats.StatsMap[worldName].ActiveOneFrameMessagesCount}");
            EditorGUILayout.LabelField($"TimeRange: {MessagesStats.StatsMap[worldName].ActiveTimeRangeMessagesCount}");
            EditorGUILayout.LabelField($"Unlimited: {MessagesStats.StatsMap[worldName].ActiveUnlimitedLifetimeMessagesCount}");
        }
        
        private void DrawRemoveAPI()
        {
            if (GUILayout.Button("Remove All")) 
                MessageBroadcaster.RemoveAllMessages(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer());
        }
    }
}