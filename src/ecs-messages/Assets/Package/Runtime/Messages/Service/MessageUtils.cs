using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageUtils
    {
        public static void DestroyImmediate(Entity messageEntity, EntityManager entityManager)
        {
            entityManager.DestroyEntity(messageEntity);
        }
    }
}