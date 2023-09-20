using System.Collections.Generic;
using System.Linq;
using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CortexDeveloper.ECSMessages.Editor.StatsWindow
{
    public class MessageBroadcasterStatsWindow : EditorWindow
    {
        private const string UxmlPath = "Assets/Package/Editor/StatsWindow/MessageBroadcasterStatsWindow.uxml";
        private const string WorldDropdown = "WorldDropdown";

        private List<string> _worldsList = new();
        private int _selectedWorld;
        private World SelectedWorld => World.All.GetWorldWithName(_worldsList[_selectedWorld]);

        private DropdownField _worldDropdown;
        
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
            
            _worldDropdown = rootVisualElement.Q<DropdownField>(WorldDropdown);
            _worldDropdown.RegisterValueChangedCallback(evt => Debug.Log(evt.newValue));
        }

        private void OnInspectorUpdate()
        {
            UpdateWorldDropdownList();
        }

        private void UpdateWorldDropdownList()
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

            _worldDropdown.choices = _worldsList;
        }
    }
}
