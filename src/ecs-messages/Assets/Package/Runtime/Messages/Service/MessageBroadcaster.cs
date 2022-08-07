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

        public static void RemoveFrom(Entity entity) => 
            MessageUtils.RemoveFrom(entity);

        public static void RemoveAllCommon() =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveAllMessagesCommand());
        
        public static void RemoveAllAttached() =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveAllAttachedMessagesCommand());

        public static void RemoveAll()
        {
            RemoveAllCommon();
            RemoveAllAttached();
        }

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
        
        public static void RemoveAttachedWithLifetime(MessageLifetime lifetime)
        {
            switch (lifetime)
            {
                case MessageLifetime.OneFrame:
                    RemoveAttachedWith<MessageLifetimeOneFrameTag>();
                    break;
                case MessageLifetime.TimeRange:
                    RemoveAttachedWith<MessageLifetimeTimeRange>();
                    break;
                case MessageLifetime.Unlimited:
                    RemoveAttachedWith<MessageLifetimeUnlimitedTag>();
                    break;
            }
        }

        public static void RemoveWith<T>() where T : struct, IComponentData =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });
        
        public static void RemoveAttachedWith<T>() where T : struct, IComponentData =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveAttachedMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveBufferWith<T>() where T : struct, IBufferElementData =>
            PrepareCommand().AliveForOneFrame().Post(new RemoveMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });

        internal static void ClearRequests() =>
            PostRequests.Clear();
    }
}