using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessageBroadcasterStatsWindow : EditorWindow
    {
        private int _selectedTab;
        private int _logsEnabled;
        
        private MessageLifetime _messageLifetimeFilter;
        
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem =>
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        [MenuItem("Tools/Message Broadcaster")]
        public static void Init()
        {
            MessageBroadcasterStatsWindow statsWindow = (MessageBroadcasterStatsWindow)GetWindow(
                typeof(MessageBroadcasterStatsWindow), 
                false, 
                "Message Broadcaster");
            
            statsWindow.Show();
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

            EditorGUILayout.LabelField($"OneFrame: {MessagesStats.ActiveOneFrameMessagesCount}");
            EditorGUILayout.LabelField($"TimeRange: {MessagesStats.ActiveTimeRangeMessagesCount}");
            EditorGUILayout.LabelField($"Unlimited: {MessagesStats.ActiveUnlimitedLifetimeMessagesCount}");
        }
        
        private void DrawRemoveAPI()
        {
            _messageLifetimeFilter = (MessageLifetime)EditorGUILayout.EnumPopup("Lifetime Filter: ", _messageLifetimeFilter);

            if (GUILayout.Button("Remove Messages by Lifetime Filter"))
                MessageBroadcaster.RemoveCommonWithLifetime(EcbSystem.CreateCommandBuffer(), _messageLifetimeFilter);

            if (GUILayout.Button("Remove All")) 
                MessageBroadcaster.RemoveAllMessages(EcbSystem.CreateCommandBuffer());
        }
    }
}