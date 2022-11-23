using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public class ExampleWorldBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            World exampleWorld = new("Example World");

            exampleWorld.GetOrCreateSystem<InitializationSystemGroup>();
            SimulationSystemGroup simulationSystemGroup = exampleWorld.GetOrCreateSystem<SimulationSystemGroup>();
            exampleWorld.GetOrCreateSystem<PresentationSystemGroup>();

            BeginSimulationEntityCommandBufferSystem beginSimulationEntityCommandBufferSystem =
                exampleWorld.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem =
                exampleWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            simulationSystemGroup.AddSystemToUpdateList(beginSimulationEntityCommandBufferSystem);
            simulationSystemGroup.AddSystemToUpdateList(endSimulationEntityCommandBufferSystem);
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(exampleWorld);

            return false;
        }
    }
}