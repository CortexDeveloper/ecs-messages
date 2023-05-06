using CortexDeveloper.ECSMessages.Service;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.UserInterfaceExample
{
    [RequireComponent(typeof(Button))]
    public class StartGameButton : MonoBehaviour
    {
        public TMP_InputField MatchDurationInputField;
        public TMP_InputField EnemiesCountInputField;
        
        private Button _button;
    
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(StartGame);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            float.TryParse(MatchDurationInputField.text, out float duration);
            int.TryParse(EnemiesCountInputField.text, out int enemies);
            
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .PostImmediate(entityManager,
                    new StartGameCommand
                    {
                        Duration = duration,
                        Enemies = enemies,
                    });
        }
    }
}
