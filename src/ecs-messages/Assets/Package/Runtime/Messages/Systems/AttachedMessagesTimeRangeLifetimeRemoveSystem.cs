using CortexDeveloper.Messages.Components;
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
                        
                        RemoveMessageComponents(entity, ecb, EntityManager);
                    }
                })
                .Run();
        }
        
        private static void RemoveMessageComponents(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            ecb.RemoveComponent<AttachedMessage>(entity);
            ecb.RemoveComponent<MessageLifetimeTimeRange>(entity);

            if (entityManager.HasComponent<MessageContextCommandTag>(entity))
                ecb.RemoveComponent<MessageContextCommandTag>(entity);
            else if (entityManager.HasComponent<MessageContextEventTag>(entity))
                ecb.RemoveComponent<MessageContextEventTag>(entity);

            if (entityManager.HasComponent<MessageUniqueTag>(entity))
                ecb.RemoveComponent<MessageUniqueTag>(entity);
        }
    }
}