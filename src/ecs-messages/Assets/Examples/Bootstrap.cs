using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Examples
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            World defaultWorld = World.DefaultGameObjectInjectionWorld;
            MessageBroadcaster.InitializeInWorld(
                defaultWorld, 
                defaultWorld.GetOrCreateSystemManaged<SimulationSystemGroup>(),
                defaultWorld.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());

            World exampleWorld = World.All.GetWorldWithName("Example World");
            MessageBroadcaster.InitializeInWorld(
                exampleWorld,
                exampleWorld.GetOrCreateSystemManaged<SimulationSystemGroup>(),
                exampleWorld.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
        }
    }
}