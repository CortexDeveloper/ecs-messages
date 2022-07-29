using CortexDeveloper.Messages.Service;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessageBroadcasterStatsWindow : EditorWindow
    {
        private MessageLifetime _messageLifetime;

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
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }

            DrawMessagesStats();
            DrawRemoveAPI();
        }

        private void DrawMessagesStats()
        {
            //TODO get valid info
            int activeMessagesCount = 0;
            int activeEventsCount = 0;
            int activeCommandsCount = 0;
            
            EditorGUILayout.LabelField($"Active Message Count: {activeMessagesCount}");
            EditorGUILayout.LabelField($"Active Events Count: {activeEventsCount}");
            EditorGUILayout.LabelField($"Active Commands Count: {activeCommandsCount}");
        }
        
        private void DrawRemoveAPI()
        {
            _messageLifetime = (MessageLifetime)EditorGUILayout.EnumPopup("Lifetime Filter: ", _messageLifetime);

            if (GUILayout.Button("Remove Messages by Lifetime Filter"))
            {
                //TODO do removing magic 
            }
            
            if (GUILayout.Button("Remove All Messages"))
            {
                //TODO do removing magic 
            }
        }
    }
}