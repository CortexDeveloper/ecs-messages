using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Burst;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeRemoveSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = MessagesEcb;
            
            new DestroyTimeRangeMessagesJob
            {
                Ecb = ecb
            }.Schedule();
            
            Dependency.Complete();
        }
    }
    
    [BurstCompile]
    internal partial struct DestroyTimeRangeMessagesJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;

        public void Execute(Entity entity, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange)
        {
            if (timeRange.LifetimeLeft <= 0)
                Ecb.DestroyEntity(entity);
        }
    }
}