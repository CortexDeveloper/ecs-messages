using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.UserInterfaceExample
{
    [RequireComponent(typeof(Button))]
    public class PauseGameButton : MonoBehaviour
    {
        private Button _button;
    
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PauseGame);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(PauseGame);
        }

        private void PauseGame()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityCommandBufferSystem ecbSystem = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

            MessageBroadcaster.PrepareMessage().AliveForOneFrame().Post(ecb, new PauseGameCommand());
        }
    }
}