using System.Collections.Generic;
using System.Linq;
using CortexDeveloper.ECSMessages.Components.Meta;
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
        private const string WorldDropdownName = "WorldDropdown";
        private const string EnableStatsToggleName = "EnableStatsToggle";
        private const string AllMessagesIntField = "AllMessagesIntField";
        private const string OneFrameMessagesIntField = "OneFrameMessagesIntField";
        private const string TimeIntervalMessagesIntField = "TimeIntervalMessagesIntField";
        private const string UnlimitedMessagesIntField = "UnlimitedMessagesIntField";

        private List<string> _worldsList = new();
        private DropdownField _worldDropdown;

        private Toggle _enableStatsToggle;

        private GroupBox _statsGroupBox;
        private IntegerField _allMessagesIntField;
        private IntegerField _oneFrameMessagesIntField;
        private IntegerField _timeIntervalMessagesIntField;
        private IntegerField _unlimitedMessagesIntField;

        private bool StatsEnabled => _enableStatsToggle.value;
        private World SelectedWorld => World.All.GetWorldWithName(_worldDropdown.value);
        private bool AnyWorldSelected => _worldDropdown.value != null;
        private bool ReadyToShowStats => Application.isPlaying && StatsEnabled && AnyWorldSelected;

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
            
            _worldDropdown = rootVisualElement.Q<DropdownField>(WorldDropdownName);
            _worldDropdown.RegisterValueChangedCallback(evt => Debug.Log(evt.newValue));
            
            _enableStatsToggle = rootVisualElement.Q<Toggle>(EnableStatsToggleName);
            
            _statsGroupBox = rootVisualElement.Q<GroupBox>("StatsGroupBox");

            _allMessagesIntField = rootVisualElement.Q<IntegerField>(AllMessagesIntField);
            _oneFrameMessagesIntField = rootVisualElement.Q<IntegerField>(OneFrameMessagesIntField);
            _timeIntervalMessagesIntField = rootVisualElement.Q<IntegerField>(TimeIntervalMessagesIntField);
            _unlimitedMessagesIntField = rootVisualElement.Q<IntegerField>(UnlimitedMessagesIntField);
        }

        private void OnInspectorUpdate()
        {
            UpdateWorldDropdownList();
            UpdateMessagesCount();
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

        private void UpdateMessagesCount()
        {
            _statsGroupBox.style.display = StatsEnabled ? DisplayStyle.Flex : DisplayStyle.None;

            if (ReadyToShowStats)
            {
                _allMessagesIntField.value = SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageTag))).CalculateEntityCount();
                _oneFrameMessagesIntField.value = SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeOneFrameTag))).CalculateEntityCount();
                _timeIntervalMessagesIntField.value = SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeTimeInterval))).CalculateEntityCount();
                _unlimitedMessagesIntField.value = SelectedWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(MessageLifetimeUnlimitedTag))).CalculateEntityCount();
            }
        }
    }
}
