using System.Collections.Generic;
using System.Linq;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.ECSMessages.Editor.StatsWindow
{
    public class MessagesStatsWindow : EditorWindow
    {
        private int _selectedTab;
        
        private List<string> _worldsList = new();
        
        private int _selectedWorld;
        private World SelectedWorld => World.All.GetWorldWithName(_worldsList[_selectedWorld]);

        private bool _enableStats;

        private static EndSimulationEntityCommandBufferSystem GetEcbSystemInWorld(World world) => 
            world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        

        [MenuItem("ECSMessages/Stats")]
        public static void Init()
        {
            MessagesStatsWindow statsWindow = (MessagesStatsWindow)GetWindow(typeof(MessagesStatsWindow), false, "Message Broadcaster Stats");
            
            statsWindow.Show(); 
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
            
            DrawParams();

            if (!_enableStats)
                return;

            DrawStats();
            Repaint();
        }
        
        private void DrawParams()
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

            _enableStats = EditorGUILayout.Toggle("Enable Stats", _enableStats);
        }

        private void DrawStats()
        {
            EditorGUILayout.Space(10f);
            DrawMessagesStats();
            EditorGUILayout.Space(25f);
            DrawRemoveAPI();
        }

        private void DrawMessagesStats()
        {
            EditorGUILayout.LabelField($"All Messages: {SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageTag))).CalculateEntityCount()}");
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField($"OneFrame: {SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeOneFrameTag))).CalculateEntityCount()}");
            EditorGUILayout.LabelField($"TimeInterval: {SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeTimeInterval))).CalculateEntityCount()}");
            EditorGUILayout.LabelField($"Unlimited: {SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeUnlimitedTag))).CalculateEntityCount()}");

        }
        
        private void DrawRemoveAPI()
        {
            if (GUILayout.Button("Remove All Messages")) 
                MessageBroadcaster.RemoveAllMessagesWith<MessageTag>(GetEcbSystemInWorld(SelectedWorld).EntityManager);
            
            if (GUILayout.Button("Remove OneFrame Messages")) 
                MessageBroadcaster.RemoveAllMessagesWith<MessageLifetimeOneFrameTag>(GetEcbSystemInWorld(SelectedWorld).EntityManager);
            
            if (GUILayout.Button("Remove TimeInterval Messages")) 
                MessageBroadcaster.RemoveAllMessagesWith<MessageLifetimeTimeInterval>(GetEcbSystemInWorld(SelectedWorld).EntityManager);
            
            if (GUILayout.Button("Remove Unlimited Messages")) 
                MessageBroadcaster.RemoveAllMessagesWith<MessageLifetimeUnlimitedTag>(GetEcbSystemInWorld(SelectedWorld).EntityManager);
        }
    }
}