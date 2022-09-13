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
    }
}