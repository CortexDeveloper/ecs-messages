using CortexDeveloper.Messages.Service;
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
            
            _selectedTab = GUILayout.Toolbar(_selectedTab, new [] {"One Frame", "Time Range", "Unlimited Time"});
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
            }
        }

        private void DrawOneFrameExamples()
        {
            // Case 1
            EditorGUILayout.LabelField("Case: You need to start game by clicking \"Start\" button.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command that we have intention to launch match of with next settings:\n" +
                                       $"{_difficulty.ToString()} difficulty level, {_matchLenght} lenght and {_enemiesCount} enemies count\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
            
            _difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty: ", _difficulty);
            _matchLenght = EditorGUILayout.FloatField("Match Length: ", _matchLenght);
            _enemiesCount = EditorGUILayout.IntField("Enemies Count: ", _enemiesCount);
            
            if (GUILayout.Button("Post Command: Start Game"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .Post(new StartMatchData
                    {
                        DifficultyLevel = _difficulty,
                        MatchLength = _matchLenght,
                        EnemiesCount = _enemiesCount
                    });
            }
            
            // Case 2
            EditorGUILayout.LabelField("Case: You need to pause game via UI button or in-game action.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-command that we have intention to pause game.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Command: Pause Game"))
            {
                MessageBroadcaster
                    .PrepareCommand()
                    .Post(new PauseGameData());
            }
            
            // Case 3
            EditorGUILayout.LabelField("Case: You need to notify somebody that character died on this frame.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event that character died.\n" +
                                       "Message will be alive only for one frame and then would be deleted.", EditorStyles.textArea);
        
            if (GUILayout.Button("Post Event: Character Died"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .Post(new CharacterDeadData { Tick = 1234567890});
            }
        }

        private void DrawTimeRangeExamples()
        {
            // Case 1 
            EditorGUILayout.LabelField("Case: Informing other non-gameplay related systems that there are two active debuffs.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event with TimeRange Lifetime type. \n" +
                                       "Message will be alive for 10 seconds and then would be deleted.", EditorStyles.textArea);

            _firstDebuff = (Debuffs)EditorGUILayout.EnumPopup("First Debuff: ", _firstDebuff);
            _secondDebuff = (Debuffs)EditorGUILayout.EnumPopup("Second Debuff: ", _secondDebuff);
            _debuffDuration = EditorGUILayout.FloatField("Debuff Duration: ", _debuffDuration);

            if (GUILayout.Button("Post Event: Debuffs State"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .WithLifeTime(_debuffDuration)
                    .PostBuffer(
                        new DebuffData{ Value = _firstDebuff },
                        new DebuffData{ Value = _secondDebuff });
            }
            
            // Case 2
            EditorGUILayout.LabelField("Case: Informing that quest available only for N seconds.", EditorStyles.helpBox);
            EditorGUILayout.LabelField("In this case we need to post message-event with TimeRange Lifetime type.\n" +
                                       "Message will be alive for N seconds and then would be deleted.", EditorStyles.textArea);
            
            _availableQuest = (Quests)EditorGUILayout.EnumPopup("Quest: ", _availableQuest);
            _questAvailabilityTime = EditorGUILayout.FloatField("Available For: ", _questAvailabilityTime);
        
            if (GUILayout.Button("Post: Quest Availability Timer"))
            {
                MessageBroadcaster
                    .PrepareEvent()
                    .WithLifeTime(_questAvailabilityTime)
                    .Post(new QuestAvailabilityData {Quest = _availableQuest});
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
                    .WithUnlimitedLifeTime()
                    .Post(new QuestCompletedData{ Value = _completedQuest});
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
                    .WithUnlimitedLifeTime()
                    .Post(new DigGoldCommandTag());
            }
        }
    }
}