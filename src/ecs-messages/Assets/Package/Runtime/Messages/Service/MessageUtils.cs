using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageUtils
    {
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
        }
    }
}