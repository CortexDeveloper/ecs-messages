using CortexDeveloper.Messages.Service;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor
{
    public class MessageBroadcasterWindow : EditorWindow
    {
        private MessageLifetime _messageLifetime;

        [MenuItem("Tools/Message Broadcaster")]
        public static void Init()
        {
            MessageBroadcasterWindow window = (MessageBroadcasterWindow)GetWindow(typeof(MessageBroadcasterWindow), false, "Message Broadcaster");
            
            window.Show();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
         
            DrawFields();
            DrawButtons();
        }

        private void DrawFields()
        {
            _messageLifetime = (MessageLifetime)EditorGUILayout.EnumPopup("Message Lifetime: ", _messageLifetime);
        }

        private void DrawButtons()
        {
            // if (GUILayout.Button("Send Command(int)"))
            // {
            //     MessageBroadcaster
            //         .PrepareCommand()
            //         .Post(new MessageIntData { Value = 123 } );
            // }
            //
            // if (GUILayout.Button("Send Event(int) with 7 seconds life time"))
            // {
            //     MessageBroadcaster
            //         .PrepareEvent()
            //         .WithLifeTime(7f)
            //         .Post(new MessageIntData { Value = 456 } );
            // }
            //
            // if (GUILayout.Button("Send Event(int) with unlimited life time"))
            // {
            //     MessageBroadcaster
            //         .PrepareEvent()
            //         .WithUnlimitedLifeTime()
            //         .Post(new MessageIntData { Value = 789 } );
            // }
            //
            // if (GUILayout.Button("Send Event(int) with buffer"))
            // {
            //     MessageIntData firstValue = new() { Value = 1 };
            //     MessageIntData secondValue = new() { Value = 2 };
            //     
            //     MessageBroadcaster
            //         .PrepareCommand()
            //         .PostBuffer(new MessageIntBuffer { Value = firstValue }, new MessageIntBuffer { Value = secondValue } );
            // }
        }
    }
}