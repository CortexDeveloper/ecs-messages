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
                defaultWorld.GetOrCreateSystem<SimulationSystemGroup>(),
                defaultWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>());

            World exampleWorld = World.All.GetWorldWithName("Example World");
            MessageBroadcaster.InitializeInWorld(
                exampleWorld,
                exampleWorld.GetOrCreateSystem<SimulationSystemGroup>(),
                exampleWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>());
        }
    }
}