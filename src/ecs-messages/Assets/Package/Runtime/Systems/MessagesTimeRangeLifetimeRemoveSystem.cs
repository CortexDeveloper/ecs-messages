using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct MessagesTimeRangeLifetimeRemoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);

            new DestroyTimeRangeMessagesJob
            {
                ECB = ecb
            }.Schedule();
            
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
    
    [BurstCompile]
    internal partial struct DestroyTimeRangeMessagesJob : IJobEntity
    {
        public EntityCommandBuffer ECB;

        public void Execute(Entity entity, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange)
        {
            if (timeRange.LifetimeLeft <= 0)
                ECB.DestroyEntity(entity);
        }
    }
}