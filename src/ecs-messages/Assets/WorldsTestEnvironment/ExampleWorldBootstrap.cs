using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public class ExampleWorldBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            World exampleWorld = new("Example World");
            
            CreateInitializationGroup(exampleWorld);
            CreateSimulationGroup(exampleWorld);
            CreatePresentationGroup(exampleWorld);
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(exampleWorld);

            return false;
        }

        private void CreateInitializationGroup(World world)
        {
            InitializationSystemGroup initGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            UpdateWorldTimeSystem updateWorldTimeSystem = world.GetOrCreateSystemManaged<UpdateWorldTimeSystem>();
            
            initGroup.AddSystemToUpdateList(updateWorldTimeSystem);
        }
        
        private void CreateSimulationGroup(World world)
        {
            SimulationSystemGroup simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            
            BeginSimulationEntityCommandBufferSystem beginSimulationEntityCommandBufferSystem =
                world.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem =
                world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            
            simGroup.AddSystemToUpdateList(beginSimulationEntityCommandBufferSystem);
            simGroup.AddSystemToUpdateList(endSimulationEntityCommandBufferSystem);
        }
        
        private void CreatePresentationGroup(World world)
        {
            world.GetOrCreateSystem<PresentationSystemGroup>();
        }
    }
}