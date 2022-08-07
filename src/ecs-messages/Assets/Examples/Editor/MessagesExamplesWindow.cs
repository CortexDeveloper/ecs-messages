using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace CortexDeveloper.Examples.Editor
{
    public class MessageBroadcasterWindow : EditorWindow
    {
        private int _selectedTab;

        // Match params
        private Difficulty _difficulty;
        private float _matchLenght;
        private int _enemiesCount;
        
        // Debuffs params
        private Debuffs _firstDebuff;
        private Debuffs _secondDebuff;
        private float _debuffDuration;
        
        // Available quest params
        private Quests _availableQuest;
        private float _questAvailabilityTime;
        
        // Completed quest params
        private Quests _completedQuest;

        [MenuItem("Tools/Messages Examples")]
        public static void Init()
        {
            MessageBroadcasterWindow window = (MessageBroadcasterWindow)GetWindow(typeof(MessageBroadcasterWindow), false, "Messages Use Case Examples");
            
            window.Show();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Works only in Play Mode. Enter Play Mode to access broadcaster API.", MessageType.Warning);
                
                return;
            }
            
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

        private void DrawOneFrameExamples()
        {
            // Case 1
            EditorGUILayout.LabelField("Case: You need to pause game via UI button or in-game action.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command that we have intention to pause game.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Command: Pause Game"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .AliveForOneFrame()
                    .Post(new PauseGameCommand());
            }
            
            // Case 2
            EditorGUILayout.LabelField("Case: You need to start game by clicking \"Start\" button.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command that we have intention to launch match with next settings:\n" +
                                       $"{_difficulty.ToString()} difficulty level, {_matchLenght} lenght and {_enemiesCount} enemies count\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
            
            _difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty: ", _difficulty);
            _matchLenght = EditorGUILayout.FloatField("Match Length: ", _matchLenght);
            _enemiesCount = EditorGUILayout.IntField("Enemies Count: ", _enemiesCount);
            
            if (GUILayout.Button("Post Unique Command: Start Game"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .AliveForOneFrame()
                    .AsUnique()
                    .Post(new StartMatchCommand
                    {
                        DifficultyLevel = _difficulty,
                        MatchLength = _matchLenght,
                        EnemiesCount = _enemiesCount
                    });
            }

            // Case 3
            EditorGUILayout.LabelField("Case: You need to notify somebody that character died on this frame.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event that character died.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Event: Character Died"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .AliveForOneFrame()
                    .Post(new CharacterDeadEvent { Tick = 1234567890 });
            }
        }

        private void DrawTimeRangeExamples()
        {
            // Case 1 
            EditorGUILayout.LabelField("Case: Informing other non-gameplay related systems that there are two active debuffs.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event with TimeRange Lifetime type. \n" +
                                       "Message will be alive for N seconds and then would be deleted.", EditorStyles.textArea);

            _firstDebuff = (Debuffs)EditorGUILayout.EnumPopup("First Debuff: ", _firstDebuff);
            _secondDebuff = (Debuffs)EditorGUILayout.EnumPopup("Second Debuff: ", _secondDebuff);
            _debuffDuration = EditorGUILayout.FloatField("Debuff Duration: ", _debuffDuration);

            if (GUILayout.Button("Post Event: Debuffs State"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .AliveForTime(_debuffDuration)
                    .PostBuffer(
                        new DebuffData{ Value = _firstDebuff },
                        new DebuffData{ Value = _secondDebuff });
            }
            
            if (GUILayout.Button("Remove Event: Debuffs State")) 
                MessageBroadcaster.RemoveBufferWith<DebuffData>();

            // Case 2 
            EditorGUILayout.LabelField("Case: Informing other non-gameplay related systems that there are two active debuffs.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("Same as previous but message is unique. \n" +
                                       "Message will be alive for N seconds and then would be deleted.", EditorStyles.textArea);

            _firstDebuff = (Debuffs)EditorGUILayout.EnumPopup("First Debuff: ", _firstDebuff);
            _secondDebuff = (Debuffs)EditorGUILayout.EnumPopup("Second Debuff: ", _secondDebuff);
            _debuffDuration = EditorGUILayout.FloatField("Debuff Duration: ", _debuffDuration);

            if (GUILayout.Button("Post Unique Event: Debuffs State"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .AsUnique()
                    .AliveForTime(_debuffDuration)
                    .PostBuffer(
                        new DebuffData{ Value = _firstDebuff },
                        new DebuffData{ Value = _secondDebuff });
            }
            
            // Case 3
            EditorGUILayout.LabelField("Case: Informing that quest available only for N seconds.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event with TimeRange Lifetime type.\n" +
                                       "Message will be alive for N seconds and then would be deleted.", EditorStyles.textArea);
            
            _availableQuest = (Quests)EditorGUILayout.EnumPopup("Quest: ", _availableQuest);
            _questAvailabilityTime = EditorGUILayout.FloatField("Available For: ", _questAvailabilityTime);
        
            if (GUILayout.Button("Post: Quest Availability Timer"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .AliveForTime(_questAvailabilityTime)
                    .Post(new QuestAvailabilityData { Quest = _availableQuest });
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
                    .PrepareEvent()
                    .AliveForUnlimitedTime()
                    .Post(new QuestCompletedEvent { Value = _completedQuest });
            }
            
            // Case 2
            EditorGUILayout.LabelField("Case: RTS player wants any free worker to start digging gold.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command.\n" +
                                       "Then listener systems should check is there any free worker.\n" +
                                       "If there is no free workers wait.\n" +
                                       "It will be alive until we manually delete it.", EditorStyles.textArea);

            if (GUILayout.Button("Post Command: Dig Gold"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .AliveForUnlimitedTime()
                    .Post(new DigGoldCommand());
            }
            
            if (GUILayout.Button("Remove Command: Dig Gold")) 
                MessageBroadcaster.RemoveWith<DigGoldCommand>();
            
            // Case 3
            EditorGUILayout.LabelField("Case: RTS player wants any free worker to start digging gold.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("Same situation but we want to have only one active instance of this command.\n" +
                                       "So, after command posting there would restriction to post one more until first is alive.\n" +
                                       "It will be alive until we manually delete it.", EditorStyles.textArea);

            if (GUILayout.Button("Post Unique Command: Dig Gold"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .AsUnique()
                    .AliveForUnlimitedTime()
                    .Post(new DigGoldCommand());
            }
        }

        private void DrawAttachedToEntityExamples()
        {
            // Case 1
            if (GUILayout.Button("Post Attached Message"))
            {
                EndSimulationEntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

                EntityManager entityManager = ecbSystem.EntityManager;

                Entity entity = entityManager.CreateEntity();
                entityManager.AddComponent<PauseGameCommand>(entity);
                
                MessageBroadcaster
                    .PrepareEvent()
                    .AttachedTo(entity)
                    .Post(new QuestCompletedEvent { Value = Quests.KillDiablo });
            }
            
            // Case 2
            if (GUILayout.Button("Post Unique Attached Message"))
            {
                EndSimulationEntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

                EntityManager entityManager = ecbSystem.EntityManager;

                Entity entity = entityManager.CreateEntity();
                entityManager.AddComponent<PauseGameCommand>(entity);
                
                MessageBroadcaster
                    .PrepareEvent()
                    .AsUnique()
                    .AttachedTo(entity)
                    .Post(new QuestCompletedEvent { Value = Quests.KillDiablo });
            }
            
            // Case 2
            if (GUILayout.Button("Post TimeRange Attached Message"))
            {
                EndSimulationEntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

                EntityManager entityManager = ecbSystem.EntityManager;

                Entity entity = entityManager.CreateEntity();
                entityManager.AddComponent<PauseGameCommand>(entity);
                
                MessageBroadcaster
                    .PrepareEvent()
                    .AliveForTime(10f)
                    .AttachedTo(entity)
                    .Post(new QuestCompletedEvent { Value = Quests.KillDiablo });
            }
        }
    }
}