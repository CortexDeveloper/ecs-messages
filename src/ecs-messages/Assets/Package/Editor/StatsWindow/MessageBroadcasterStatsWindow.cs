using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CortexDeveloper.ECSMessages.Editor.StatsWindow
{
    public class MessageBroadcasterStatsWindow : EditorWindow
    {
        private const string UxmlPath = "Assets/Package/Editor/StatsWindow/MessageBroadcasterStatsWindow.uxml";

        [MenuItem("ECSMessages/MessageBroadcasterStatsWindow")]
        public static void ShowExample()
        {
            MessageBroadcasterStatsWindow wnd = GetWindow<MessageBroadcasterStatsWindow>();
            wnd.titleContent = new GUIContent("MessageBroadcasterStatsWindow");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            VisualElement buildTree = visualTree.Instantiate();
        
            root.Add(buildTree);
        }
    }
}
