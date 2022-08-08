using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageUtils
    {
        internal static void RemoveFrom(Entity entity)
        {
            EndSimulationEntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
                
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
            EntityManager entityManager = ecbSystem.EntityManager;
            
            RemoveMessageComponents(entity, ecb, entityManager);
        }
        
        internal static void RemoveMessageComponents(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            RemoveMarkers(entity, ecb, entityManager);
            RemoveLifeTime(entity, ecb, entityManager);
            RemoveContext(entity, ecb, entityManager);
            RemoveUnique(entity, ecb, entityManager);
        }

        private static void RemoveMarkers(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            if (entityManager.HasComponent<MessageTag>(entity))
            {
                ecb.RemoveComponent<MessageTag>(entity);
            }
            else if (entityManager.HasComponent<AttachedMessage>(entity))
            {
                AttachedMessage attachedMessage = entityManager.GetComponentData<AttachedMessage>(entity); 
                
                ecb.RemoveComponent(attachedMessage.TargetEntity, attachedMessage.ComponentType);
                ecb.RemoveComponent<AttachedMessage>(entity);
            }
        }
        
        private static void RemoveLifeTime(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            if (entityManager.HasComponent<MessageLifetimeOneFrameTag>(entity))
                ecb.RemoveComponent<MessageLifetimeOneFrameTag>(entity);
            else if (entityManager.HasComponent<MessageLifetimeTimeRange>(entity))
                ecb.RemoveComponent<MessageLifetimeTimeRange>(entity);
            else if (entityManager.HasComponent<MessageLifetimeUnlimitedTag>(entity))
                ecb.RemoveComponent<MessageLifetimeUnlimitedTag>(entity);
        }
        
        private static void RemoveContext(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            if (entityManager.HasComponent<MessageContextCommandTag>(entity))
                ecb.RemoveComponent<MessageContextCommandTag>(entity);
            else if (entityManager.HasComponent<MessageContextEventTag>(entity))
                ecb.RemoveComponent<MessageContextEventTag>(entity);
        }
        
        private static void RemoveUnique(Entity entity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            if (entityManager.HasComponent<MessageUniqueTag>(entity))
                ecb.RemoveComponent<MessageUniqueTag>(entity);
        }
    }
}