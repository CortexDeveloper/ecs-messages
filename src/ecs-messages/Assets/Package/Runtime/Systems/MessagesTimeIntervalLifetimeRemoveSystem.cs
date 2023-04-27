using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct MessagesTimeIntervalLifetimeRemoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);

            new DestroyTimeIntervalMessagesJob
            {
                ECB = ecb
            }.Schedule();
            
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
    
    [BurstCompile]
    internal partial struct DestroyTimeIntervalMessagesJob : IJobEntity
    {
        public EntityCommandBuffer ECB;

        public void Execute(Entity entity, in MessageTag messageTag, in MessageLifetimeTimeInterval timeInterval)
        {
            if (timeInterval.LifetimeLeft <= 0)
                ECB.DestroyEntity(entity);
        }
    }
}