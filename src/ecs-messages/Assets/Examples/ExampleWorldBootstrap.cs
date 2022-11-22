using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public class ExampleWorldBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            World exampleWorld = new("Example World");

            exampleWorld.GetOrCreateSystem<InitializationSystemGroup>();
            exampleWorld.GetOrCreateSystem<SimulationSystemGroup>();
            exampleWorld.GetOrCreateSystem<PresentationSystemGroup>();
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(exampleWorld);

            return false;
        }
    }
}