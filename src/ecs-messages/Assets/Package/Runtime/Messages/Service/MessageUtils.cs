using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageUtils
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;

        private static EndSimulationEntityCommandBufferSystem EcbSystem =>
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        internal static void Destroy(Entity messageEntity, EntityCommandBuffer ecb, EntityManager entityManager)
        {
            if (entityManager.HasComponent<MessageTag>(messageEntity))
            {
                if (entityManager.HasComponent<AttachedMessageContent>(messageEntity))
                {
                    AttachedMessageContent attachedMessageContent = entityManager.GetComponentData<AttachedMessageContent>(messageEntity);
                
                    ecb.RemoveComponent(attachedMessageContent.TargetEntity, attachedMessageContent.ComponentType);
                    ecb.DestroyEntity(messageEntity);
                }
                
                ecb.DestroyEntity(messageEntity);
            }
            else
            {
                MessagesLogger.LogWarning($"Cannot destroy message. Entity ({messageEntity}) doesn't contain message meta components.");
            }
        }
    }
}