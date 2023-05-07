using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;

namespace Samples.UserInterfaceExample
{
    public class UIExampleBootstrap : MonoBehaviour
    {
        private static World _world;
        private SimulationSystemGroup _systemGroup;

        private void Awake()
        {
            InitializeMessageBroadcaster();
            CreateExampleSystems();
        }

        private void OnDestroy()
        {
            MessageBroadcaster.DisposeFromWorld(_world);
        }
        
        private void InitializeMessageBroadcaster()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _systemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();

            MessageBroadcaster.InitializeInWorld(_world, _systemGroup);
        }

        private void CreateExampleSystems()
        {
            _systemGroup.AddSystemToUpdateList(_world.CreateSystem<StartGameSystem>());
            _systemGroup.AddSystemToUpdateList(_world.CreateSystem<PauseGameSystem>());
        }
    }
}