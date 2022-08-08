using System.Collections.Generic;
using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static readonly HashSet<ComponentType> PostRequests = new();

        public static MessageBuilder PrepareEvent() =>
            new() { Context = MessageContext.Event };

        public static MessageBuilder PrepareCommand() =>
            new() { Context = MessageContext.Command };

        public static void RemoveAll() =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveAllMessagesCommand());

        public static void RemoveWith<T>() where T : struct, IComponentData =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveCommonWithLifetime(MessageLifetime lifetime)
        {
            switch (lifetime)
            {
                case MessageLifetime.OneFrame:
                    RemoveWith<MessageLifetimeOneFrameTag>();
                    break;
                case MessageLifetime.TimeRange:
                    RemoveWith<MessageLifetimeTimeRange>();
                    break;
                case MessageLifetime.Unlimited:
                    RemoveWith<MessageLifetimeUnlimitedTag>();
                    break;
            }
        }

        internal static void ClearRequests() =>
            PostRequests.Clear();
    }
}