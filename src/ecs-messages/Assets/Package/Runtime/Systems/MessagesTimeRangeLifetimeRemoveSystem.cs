using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeRemoveSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer.ParallelWriter ecb = MessagesEcb.AsParallelWriter();

            Entities
                .ForEach((Entity entity, int entityInQueryIndex, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange) =>
                {
                    if (timeRange.LifetimeLeft <= 0)
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();
            
            Dependency.Complete();
        }
    }
}