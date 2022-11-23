using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeTimerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

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
                .Schedule();

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}