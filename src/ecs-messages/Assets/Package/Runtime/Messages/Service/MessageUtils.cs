using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageUtils
    {
        internal static void Destroy(Entity messageEntity, EntityCommandBuffer ecb)
        {
            ecb.DestroyEntity(messageEntity);
        }

        public static void DestroyImmediate(Entity messageEntity, EntityManager entityManager)
        {
            entityManager.DestroyEntity(messageEntity);
        }
    }
}