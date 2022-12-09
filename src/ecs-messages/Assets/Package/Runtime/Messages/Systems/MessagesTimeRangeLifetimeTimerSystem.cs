using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeTimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;

            Entities
                .ForEach((Entity entity, ref MessageLifetimeTimeRange timeRange, in MessageTag messageTag) =>
                {
                    timeRange.LifetimeLeft -= deltaTime;
                }).ScheduleParallel();
            
            Dependency.Complete();
        }
    }
}