using System.Collections.Generic;
using System.Linq;
using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Examples.Editor
{
    public class MessagesExamplesWindow : EditorWindow
    {
        // Available quest params
        private Quests _availableQuest;
        private float _questAvailabilityTime;
        
        // Completed quest params
        private Quests _completedQuest;

        private int _selectedTab;
        
        private List<string> _worldsList = new();

        private int _selectedWorld;
        private World SelectedWorld => World.All.GetWorldWithName(_worldsList[_selectedWorld]);

        private static EndSimulationEntityCommandBufferSystem GetEcbSystemInWorld(World world) => 
            world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        [MenuItem("Tools/Messages Examples")]
        public static void Init()
        {
            MessagesExamplesWindow window = (MessagesExamplesWindow)GetWindow(typeof(MessagesExamplesWindow), false, "Messages Use Case Examples");
            
            window.Show();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
            
            DrawWorldPopup();
            
            _selectedTab = GUILayout.Toolbar(_selectedTab, new [] {"One Frame", "Time Range", "Unlimited Time", "Attached"});
            switch (_selectedTab)
            {
                case 0:
                    DrawOneFrameExamples();
                    break;
                case 1:
                    DrawTimeRangeExamples();
                    break;
                case 2:
                    DrawUnlimitedLifetimeExamples();
                    break;
                case 3:
                    DrawAttachedToEntityExamples();
                    break;
            }
        }

        private void DrawWorldPopup()
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
            _selectedWorld = EditorGUILayout.Popup("World", _selectedWorld, _worldsList.ToArray());
        }

        private void DrawOneFrameExamples()
        {
            // Case 1
            EditorGUILayout.LabelField("Case: You need to pause game via UI button or in-game action.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command that we have intention to pause game.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Command: Pause Game"))
            {
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForOneFrame()
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new PauseGameCommand());
            }

            // Case 2
            EditorGUILayout.LabelField("Case: You need to notify somebody that character died on this frame.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event that character died.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Event: Character Died"))
            {
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForOneFrame()
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new CharacterDeadEvent { Tick = 1234567890 });
            }
        }

        private void DrawTimeRangeExamples()
        {
            // Case 1
            EditorGUILayout.LabelField("Case: Informing that quest available only for N seconds.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event with TimeRange Lifetime type.\n" +
                                       "Message will be alive for N seconds and then would be deleted.", EditorStyles.textArea);
            
            _availableQuest = (Quests)EditorGUILayout.EnumPopup("Quest: ", _availableQuest);
            _questAvailabilityTime = EditorGUILayout.FloatField("Available For: ", _questAvailabilityTime);
        
            if (GUILayout.Button("Post: Quest Availability Timer"))
            {
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForSeconds(_questAvailabilityTime)
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new QuestAvailabilityData { Quest = _availableQuest });
            }
        }

        private void DrawUnlimitedLifetimeExamples()
        {
            // Case 1
            EditorGUILayout.LabelField("Case: Notify that quest is completed.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event. \n" +
                                       "It will be alive until we manually delete it.", EditorStyles.textArea);
        
            _completedQuest = (Quests)EditorGUILayout.EnumPopup("Quest: ", _completedQuest);

            if (GUILayout.Button("Post Event: Quest Completed"))
            {
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForUnlimitedTime()
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new QuestCompletedEvent { Value = _completedQuest });
            }
        }

        private void DrawAttachedToEntityExamples()
        {
            // Case 1
            if (GUILayout.Button("Post Attached Message"))
            {
                EndSimulationEntityCommandBufferSystem ecbSystem = SelectedWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
                EntityManager entityManager = ecbSystem.EntityManager;

                Entity entity = entityManager.CreateEntity();
                entityManager.SetName(entity, "MessageHolder");
                entityManager.AddComponent<PauseGameCommand>(entity);
                
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForOneFrame()
                    .AttachedTo(entity)
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new QuestCompletedEvent { Value = Quests.KillDiablo });
            }

            // Case 2
            if (GUILayout.Button("Post TimeRange Attached Message"))
            {
                EndSimulationEntityCommandBufferSystem ecbSystem = SelectedWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
                EntityManager entityManager = ecbSystem.EntityManager;

                Entity entity = entityManager.CreateEntity();
                entityManager.SetName(entity, "MessageHolder");
                entityManager.AddComponent<PauseGameCommand>(entity);
                
                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForSeconds(10f)
                    .AttachedTo(entity)
                    .Post(GetEcbSystemInWorld(SelectedWorld).CreateCommandBuffer(), new QuestCompletedEvent { Value = Quests.KillDiablo });
            }
        }
    }
}