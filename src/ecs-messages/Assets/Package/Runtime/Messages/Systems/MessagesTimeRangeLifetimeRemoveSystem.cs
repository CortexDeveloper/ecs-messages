using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class MessagesTimeRangeLifetimeRemoveSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), typeof(MessageLifetimeTimeRange)));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange) =>
                {
                    if (timeRange.LifetimeLeft <= 0f)
                        ecb.DestroyEntity(entity);
                })
                .Schedule();

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}