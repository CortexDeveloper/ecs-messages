using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Examples
{
    public class ServiceBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            World defaultWorld = World.DefaultGameObjectInjectionWorld;
            MessageBroadcaster.InitializeInWorld(defaultWorld, defaultWorld.GetOrCreateSystemManaged<SimulationSystemGroup>());

            World exampleWorld = World.All.GetWorldWithName("Example World");
            MessageBroadcaster.InitializeInWorld(exampleWorld, exampleWorld.GetOrCreateSystemManaged<SimulationSystemGroup>());
        }
    }
}