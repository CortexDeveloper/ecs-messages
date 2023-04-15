using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.ECSMessages.Editor.TestWindow
{
    public class TestAPIWindow : EditorWindow
    {
        private const string ECS_MESSAGES_TESTS_WORLD_KEY = "ECS_MESSAGES_TESTS_WORLD_KEY";
        
        private string _worldName = "Default World";
        
        [MenuItem("ECSMessages/Tests")]
        public static void Init()
        {
            TestAPIWindow window = (TestAPIWindow)GetWindow(
                typeof(TestAPIWindow), 
                false, 
                "ECS Messages Test API");
            
            window.Show();
        }

        public void OnGUI()
        {
            DrawSettings();
            Repaint();
        }

        private void DrawSettings()
        {
            _worldName = EditorGUILayout.TextField("World to run tests in: ", _worldName);
            
            EditorGUILayout.LabelField($"Saved world: {EditorPrefs.GetString(ECS_MESSAGES_TESTS_WORLD_KEY, "Default World")}");

            if (GUILayout.Button("Save chosen world for tests"))
            {
                EditorPrefs.SetString(ECS_MESSAGES_TESTS_WORLD_KEY, _worldName);
            }
        }
    }
}