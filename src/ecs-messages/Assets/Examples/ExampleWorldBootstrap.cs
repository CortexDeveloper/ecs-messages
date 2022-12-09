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
            InitializationSystemGroup initGroup = world.GetOrCreateSystem<InitializationSystemGroup>();

            UpdateWorldTimeSystem updateWorldTimeSystem = world.GetOrCreateSystem<UpdateWorldTimeSystem>();
            
            initGroup.AddSystemToUpdateList(updateWorldTimeSystem);
        }
        
        private void CreateSimulationGroup(World world)
        {
            SimulationSystemGroup simGroup = world.GetOrCreateSystem<SimulationSystemGroup>();
            
            BeginSimulationEntityCommandBufferSystem beginSimulationEntityCommandBufferSystem =
                world.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem =
                world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            simGroup.AddSystemToUpdateList(beginSimulationEntityCommandBufferSystem);
            simGroup.AddSystemToUpdateList(endSimulationEntityCommandBufferSystem);
        }
        
        private void CreatePresentationGroup(World world)
        {
            world.GetOrCreateSystem<PresentationSystemGroup>();
        }
    }
}