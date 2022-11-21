using System;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.SystemGroups;
using CortexDeveloper.Messages.Systems;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static NativeList<ComponentType> PostRequests = new(Allocator.Persistent);

        private static bool _isPostRequestsDisposed;

        public static void Initialize(World world)
        {
            MessagesSystemGroup messagesSystemGroup = world.GetOrCreateSystem<MessagesSystemGroup>();
        
            MessagesDateTimeSystem dateTimeSystem = world.GetOrCreateSystem<MessagesDateTimeSystem>();
            MessagesOneFrameLifetimeSystem oneFrameLifetimeSystem = world.GetOrCreateSystem<MessagesOneFrameLifetimeSystem>();
            MessagesPostRequestsHandleSystem postRequestsHandleSystem = world.GetOrCreateSystem<MessagesPostRequestsHandleSystem>();
            MessagesRemoveAllCommandListenerSystem removeAllCommandListenerSystem = world.GetOrCreateSystem<MessagesRemoveAllCommandListenerSystem>();
            MessagesRemoveByComponentCommandListenerSystem removeByComponentCommandListenerSystem = world.GetOrCreateSystem<MessagesRemoveByComponentCommandListenerSystem>();
            MessagesTimeRangeLifetimeRemoveSystem timeRangeLifetimeRemoveSystem = world.GetOrCreateSystem<MessagesTimeRangeLifetimeRemoveSystem>();
            MessagesTimeRangeLifetimeTimerSystem timeRangeLifetimeTimerSystem = world.GetOrCreateSystem<MessagesTimeRangeLifetimeTimerSystem>();
            
            messagesSystemGroup.AddSystemToUpdateList(dateTimeSystem);
            messagesSystemGroup.AddSystemToUpdateList(oneFrameLifetimeSystem);
            messagesSystemGroup.AddSystemToUpdateList(postRequestsHandleSystem);
            messagesSystemGroup.AddSystemToUpdateList(removeAllCommandListenerSystem);
            messagesSystemGroup.AddSystemToUpdateList(removeByComponentCommandListenerSystem);
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeRemoveSystem);
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeTimerSystem);
        }

        public static MessageBuilder PrepareEvent(EntityCommandBuffer ecb) =>
            ecb.PrepareEvent();

        public static MessageBuilder PrepareCommand(EntityCommandBuffer ecb) =>
            ecb.PrepareCommand();

        public static void RemoveMessage(EntityCommandBuffer ecb, EntityManager entityManager, Entity entity) =>
            MessageUtils.Destroy(entity, ecb, entityManager);

        public static void RemoveAllMessages(EntityCommandBuffer ecb) =>
            PrepareCommand(ecb).AliveForOneFrame().Post(new RemoveAllMessagesCommand());

        public static void RemoveAllMessagesWith<T>(EntityCommandBuffer ecb) where T : struct, IComponentData =>
            PrepareCommand(ecb).AliveForOneFrame().Post(new RemoveMessagesByComponentCommand 
                { ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveCommonWithLifetime(EntityCommandBuffer ecb, MessageLifetime lifetime)
        {
            switch (lifetime)
            {
                case MessageLifetime.OneFrame:
                    RemoveAllMessagesWith<MessageLifetimeOneFrameTag>(ecb);
                    break;
                case MessageLifetime.TimeRange:
                    RemoveAllMessagesWith<MessageLifetimeTimeRange>(ecb);
                    break;
                case MessageLifetime.Unlimited:
                    RemoveAllMessagesWith<MessageLifetimeUnlimitedTag>(ecb);
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