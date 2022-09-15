using System;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static NativeList<ComponentType> PostRequests = new(Allocator.Persistent);

        internal static readonly int RandomSeed = new Random().Next(int.MinValue, int.MaxValue);
        
        private static bool _isPostRequestsDisposed;

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

        public static void Dispose()
        {
            PostRequests.Dispose();

            _isPostRequestsDisposed = true;
        }

        internal static void ClearRequests()
        {
            if (_isPostRequestsDisposed || !PostRequests.IsCreated)
                return;
            
            PostRequests.Clear();
        }
    }
}