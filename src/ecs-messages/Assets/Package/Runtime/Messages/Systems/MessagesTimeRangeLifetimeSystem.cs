using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))] 
    public partial class MessagesTimeRangeLifetimeSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            float deltaTime = Time.DeltaTime;
            
            Entities
                .ForEach((Entity entity, ref MessageLifetimeTimeRange timeRange) =>
                {
                    MessageLifetimeTimeRange messageTimeRange = timeRange;
                    messageTimeRange.LifetimeLeft -= deltaTime;
                    timeRange = messageTimeRange;

                    if (timeRange.LifetimeLeft <= 0f) 
                        ecb.DestroyEntity(entity);
                })
                .Schedule();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}