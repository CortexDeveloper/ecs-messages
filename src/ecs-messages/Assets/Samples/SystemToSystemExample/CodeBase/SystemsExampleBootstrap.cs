using CortexDeveloper.ECSMessages.Service;
using Samples.UserInterfaceExample;
using Unity.Entities;
using UnityEngine;

namespace Samples.SystemToSystemExample
{
    public class SystemsExampleBootstrap : MonoBehaviour
    {
        private static World _world;
        private SimulationSystemGroup _systemGroup;

        private void Awake()
        {
            InitializeMessageBroadcaster();
            CreateExampleSystems();
        }

        private void InitializeMessageBroadcaster()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _systemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();

            MessageBroadcaster.InitializeInWorld(_world, _systemGroup);
        }

        private void CreateExampleSystems()
        {
            _systemGroup.AddSystemToUpdateList(_world.CreateSystem<CountdownSystem>());
            _systemGroup.AddSystemToUpdateList(_world.CreateSystem<CountdownAnalyticsTrackingSystem>());
        }
    }
}