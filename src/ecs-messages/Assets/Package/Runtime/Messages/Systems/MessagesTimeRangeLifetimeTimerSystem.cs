using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeTimerSystem : MessagesBaseSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate(GetEntityQuery(typeof(MessageLifetimeTimeRange)));
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities
                .ForEach((Entity entity, ref MessageLifetimeTimeRange timeRange) =>
                {
                    MessageLifetimeTimeRange messageTimeRange = timeRange;
                    messageTimeRange.LifetimeLeft -= deltaTime;
                    timeRange = messageTimeRange;
                })
                .Run();
        }
    }
}