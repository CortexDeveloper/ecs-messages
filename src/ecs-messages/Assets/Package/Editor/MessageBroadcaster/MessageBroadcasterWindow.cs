using System;
using System.Collections.Generic;
using System.Reflection;
using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessageBroadcasterWindow : EditorWindow
    {
        private const string LogsEnabledKey = "ECS_MESSAGES_LOGS_ENABLED";
        
        private int _selectedTab;
        private int _logsEnabled;
        
        private MessageLifetime _messageLifetimeFilter;

        public int PostRequestsCount
        {
            get
            {
                FieldInfo postRequestsFieldInfo = typeof(MessageBroadcaster).GetField(
                    "PostRequests", 
                    BindingFlags.NonPublic | BindingFlags.Static);

                HashSet<ComponentType> value = postRequestsFieldInfo.GetValue(null) as HashSet<ComponentType>;
                    
                return value.Count;
            }
        }

        [MenuItem("Tools/Message Broadcaster")]
        public static void Init()
        {
            MessageBroadcasterWindow window = (MessageBroadcasterWindow)GetWindow(
                typeof(MessageBroadcasterWindow), 
                false, 
                "Message Broadcaster");
            
            window.Show();
        }

        public void OnGUI()
        {
            DrawStats();
            Repaint();
        }

        private void DrawStats()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
            
            DrawMessagesStats();
            EditorGUILayout.Space(25f);
            DrawRemoveAPI();
        }

        private void DrawMessagesStats()
        {
            EditorGUILayout.LabelField($"Messages: {MessagesStats.ActiveMessagesCount}");
            EditorGUILayout.LabelField($"Attached Messages: {MessagesStats.ActiveAttachedMessagesCount}");
            EditorGUILayout.LabelField($"Events: {MessagesStats.ActiveEventsCount}");
            EditorGUILayout.LabelField($"Commands: {MessagesStats.ActiveCommandsCount}");
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField($"Unique: {MessagesStats.ActiveUniqueCount}");
            EditorGUILayout.LabelField($"Post Requests: {PostRequestsCount}");
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField($"OneFrame: {MessagesStats.ActiveOneFrameMessagesCount}");
            EditorGUILayout.LabelField($"TimeRange: {MessagesStats.ActiveTimeRangeMessagesCount}");
            EditorGUILayout.LabelField($"Unlimited: {MessagesStats.ActiveUnlimitedLifetimeMessagesCount}");
        }
        
        private void DrawRemoveAPI()
        {
            _messageLifetimeFilter = (MessageLifetime)EditorGUILayout.EnumPopup("Lifetime Filter: ", _messageLifetimeFilter);

            if (GUILayout.Button("Remove Messages by Lifetime Filter"))
                MessageBroadcaster.RemoveCommonWithLifetime(_messageLifetimeFilter);

            if (GUILayout.Button("Remove All")) 
                MessageBroadcaster.RemoveAll();
        }
    }
}