using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.UserInterfaceExample.CodeBase
{
    [RequireComponent(typeof(Button))]
    public class StartGameButton : MonoBehaviour
    {
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
            World world = World.DefaultGameObjectInjectionWorld;
            EntityCommandBufferSystem ecbSystem = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .Post(ecb,
                    new StartGameCommand
                    {
                        Enemies = 999,
                        Duration = 600f
                    });
        }
    }
}
