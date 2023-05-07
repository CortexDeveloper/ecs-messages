using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;

namespace Samples.UserInterfaceExample
{
    public class UIExampleBootstrap : MonoBehaviour
    {
        private static World _world;
        private SimulationSystemGroup _simulationSystemGroup;
        private LateSimulationSystemGroup _lateSimulationSystemGroup;

        private void Awake()
        {
            InitializeMessageBroadcaster();
            CreateExampleSystems();
        }

        private void InitializeMessageBroadcaster()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            _lateSimulationSystemGroup = _world.GetOrCreateSystemManaged<LateSimulationSystemGroup>();

            MessageBroadcaster.InitializeInWorld(_world, _lateSimulationSystemGroup);
        }

        private void CreateExampleSystems()
        {
            _simulationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<StartGameSystem>());
            _simulationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<PauseGameSystem>());
        }
    }
}