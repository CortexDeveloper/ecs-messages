using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeRemoveSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = MessagesEcb;

            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange) =>
                {
                    if (timeRange.LifetimeLeft <= 0)
                        ecb.DestroyEntity(entity);
                }).Run();
        }
    }
}