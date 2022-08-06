using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class AttachedMessagesTimeRangeLifetimeRemoveSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<AttachedMessage>(), typeof(MessageLifetimeTimeRange)));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

            Entities
                .WithoutBurst()
                .ForEach((Entity entity, in AttachedMessage attachedMessage, in MessageLifetimeTimeRange timeRange) =>
                {
                    if (timeRange.LifetimeLeft <= 0f)
                    {
                        ecb.RemoveComponent(entity, attachedMessage.ComponentType);
                        
                        MessageUtils.RemoveMessageComponents(entity, ecb, EntityManager);
                    }
                })
                .Run();
        }
    }
}