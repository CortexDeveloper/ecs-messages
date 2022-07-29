using CortexDeveloper.Messages.Service;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessageBroadcasterStatsWindow : EditorWindow
    {
        private MessageLifetime _messageLifetimeFilter;

        [MenuItem("Tools/Message Broadcaster Stats")]
        public static void Init()
        {
            MessageBroadcasterStatsWindow statsWindow = (MessageBroadcasterStatsWindow)GetWindow(
                typeof(MessageBroadcasterStatsWindow), 
                false, 
                "Message Broadcaster Stats");
            
            statsWindow.Show();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }

            DrawMessagesStats();
            EditorGUILayout.Space(25f);
            DrawRemoveAPI();

            Repaint();
        }

        private void DrawMessagesStats()
        {
            EditorGUILayout.LabelField($"Active Messages(Events + Commands): {MessagesStats.ActiveMessagesCount}");
            EditorGUILayout.LabelField($"Active Events: {MessagesStats.ActiveEventsCount}");
            EditorGUILayout.LabelField($"Active Commands: {MessagesStats.ActiveCommandsCount}");
            EditorGUILayout.LabelField($"Active OneFrame Messages: {MessagesStats.ActiveOneFrameMessagesCount}");
            EditorGUILayout.LabelField($"Active TimeRange Messages: {MessagesStats.ActiveTimeRangeMessagesCount}");
            EditorGUILayout.LabelField($"Active UnlimitedLifetime Messages: {MessagesStats.ActiveUnlimitedLifetimeMessagesCount}");
        }

        private void DrawRemoveAPI()
        {
            _messageLifetimeFilter = (MessageLifetime)EditorGUILayout.EnumPopup("Lifetime Filter: ", _messageLifetimeFilter);

            if (GUILayout.Button("Remove Messages by Lifetime Filter"))
                MessageBroadcaster.RemoveWithLifetime(_messageLifetimeFilter);

            if (GUILayout.Button("Remove All Messages")) 
                MessageBroadcaster.RemoveAll();
        }
    }
}