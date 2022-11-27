using CortexDeveloper.Tests;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace CortexDeveloper.Messages.Editor.TestWindow
{
    public class TestAPIWindow : EditorWindow
    {
        private string _worldName = "Default World";
        
        [MenuItem("DOTS/ECSMessages/Tests")]
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
            
            EditorGUILayout.LabelField($"Saved world: {EditorPrefs.GetString(TestsConstants.TESTS_WORLD_KEY, "Default World")}");

            if (GUILayout.Button("Save chosen world for tests"))
            {
                EditorPrefs.SetString(TestsConstants.TESTS_WORLD_KEY, _worldName);
            }

            if (GUILayout.Button($"Run all tests in {_worldName}"))
            {
                EditorPrefs.SetString(TestsConstants.TESTS_WORLD_KEY, _worldName);
                RunAll();
            }
        }

        private void RunAll()
        {
            TestRunnerApi testRunnerApi = CreateInstance<TestRunnerApi>();
            Filter filter = new() { testMode = TestMode.PlayMode };
            testRunnerApi.Execute(new ExecutionSettings(filter));
        }
    }
}