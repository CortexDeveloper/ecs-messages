using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.SystemGroups;
using CortexDeveloper.Messages.Systems;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, EntityCommandBufferSystem ecbSystem)
        {
            ComponentSystemGroup systemGroup = parentSystemGroup;
            MessagesSystemGroup messagesSystemGroup = world.GetOrCreateSystem<MessagesSystemGroup>();
            
            systemGroup.AddSystemToUpdateList(messagesSystemGroup);

            MessagesDateTimeSystem dateTimeSystem = world.GetOrCreateSystem<MessagesDateTimeSystem>();
            MessagesOneFrameLifetimeSystem oneFrameLifetimeSystem = world.GetOrCreateSystem<MessagesOneFrameLifetimeSystem>();
            MessagesRemoveByComponentCommandListenerSystem removeByComponentCommandListenerSystem = world.GetOrCreateSystem<MessagesRemoveByComponentCommandListenerSystem>();
            MessagesTimeRangeLifetimeRemoveSystem timeRangeLifetimeRemoveSystem = world.GetOrCreateSystem<MessagesTimeRangeLifetimeRemoveSystem>();
            MessagesTimeRangeLifetimeTimerSystem timeRangeLifetimeTimerSystem = world.GetOrCreateSystem<MessagesTimeRangeLifetimeTimerSystem>();
            MessagesStatsSystem statsSystem = world.GetOrCreateSystem<MessagesStatsSystem>();

            messagesSystemGroup.AddSystemToUpdateList(statsSystem);
            messagesSystemGroup.AddSystemToUpdateList(dateTimeSystem);
            messagesSystemGroup.AddSystemToUpdateList(oneFrameLifetimeSystem);
            messagesSystemGroup.AddSystemToUpdateList(removeByComponentCommandListenerSystem);
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeRemoveSystem);
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeTimerSystem);

            MessagesStats.StatsMap.Add(world.Name, new Stats());
        }

        public static void Dispose() => 
            MessagesStats.StatsMap.Clear();

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName = default) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityCommandBuffer ecb) where T : struct, IComponentData =>
            PrepareMessage().AliveForOneFrame().Post(ecb, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}