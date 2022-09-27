using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class EntityCommandBufferExtensions
    {
        public static MessageBuilder PrepareEvent(this EntityCommandBuffer ecb) =>
            new()
            {
                Ecb = ecb,
                Context = MessageContext.Event
            };

        public static MessageBuilder PrepareCommand(this EntityCommandBuffer ecb) =>
            new()
            {
                Ecb = ecb,
                Context = MessageContext.Command
            };

        public static void RemoveMessage(this EntityCommandBuffer ecb, EntityManager entityManager, Entity entity) =>
            MessageBroadcaster.RemoveMessage(ecb, entityManager, entity);

        public static void RemoveAllMessages(this EntityCommandBuffer ecb) =>
            MessageBroadcaster.RemoveAllMessages(ecb);

        public static void RemoveAllMessagesWith<T>(this EntityCommandBuffer ecb) where T : struct, IComponentData, IMessageComponent =>
            MessageBroadcaster.RemoveAllMessagesWith<T>(ecb);
    }
}