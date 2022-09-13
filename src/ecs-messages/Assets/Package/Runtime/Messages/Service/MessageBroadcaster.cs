using System.Collections.Generic;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static readonly HashSet<ComponentType> PostRequests = new();

        public static MessageBuilder PrepareEvent(EntityCommandBuffer ecb) =>
            new()
            {
                Ecb = ecb,
                Context = MessageContext.Event
            };

        public static MessageBuilder PrepareCommand(EntityCommandBuffer ecb) =>
            new()
            {
                Ecb = ecb,
                Context = MessageContext.Command
            };

        public static void RemoveAll(EntityCommandBuffer ecb) =>
            PrepareCommand(ecb).AliveForOneFrame().Post(new RemoveAllMessagesCommand());

        public static void RemoveWith<T>(EntityCommandBuffer ecb) where T : struct, IComponentData =>
            PrepareCommand(ecb).AliveForOneFrame().Post(new RemoveMessagesByComponentCommand 
                { ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveCommonWithLifetime(EntityCommandBuffer ecb, MessageLifetime lifetime)
        {
            switch (lifetime)
            {
                case MessageLifetime.OneFrame:
                    RemoveWith<MessageLifetimeOneFrameTag>(ecb);
                    break;
                case MessageLifetime.TimeRange:
                    RemoveWith<MessageLifetimeTimeRange>(ecb);
                    break;
                case MessageLifetime.Unlimited:
                    RemoveWith<MessageLifetimeUnlimitedTag>(ecb);
                    break;
            }
        }

        internal static void ClearRequests() =>
            PostRequests.Clear();
    }
}