using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeTimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities
                .ForEach((Entity entity, ref MessageLifetimeTimeRange timeRange, in MessageTag messageTag) =>
                {
                    MessageLifetimeTimeRange messageTimeRange = timeRange;
                    messageTimeRange.LifetimeLeft -= deltaTime;
                    timeRange = messageTimeRange;
                }).Run();
        }
    }
}