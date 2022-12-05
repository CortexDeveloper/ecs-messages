using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.SystemGroups;
using CortexDeveloper.Messages.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static readonly SharedStatic<Random> Random = SharedStatic<Random>.GetOrCreate<RandomKey, Random>();

        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, EntityCommandBufferSystem ecbSystem)
        {
            if (Random.Data.state == 0)
                Random.Data.InitState(1);
            
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
            messagesSystemGroup.AddSystemToUpdateList(oneFrameLifetimeSystem.Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeRemoveSystem.Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(timeRangeLifetimeTimerSystem);
            messagesSystemGroup.AddSystemToUpdateList(removeByComponentCommandListenerSystem.Construct(ecbSystem));

            MessagesStats.StatsMap.Add(world.Name, new Stats());
        }

        public static void Dispose() => 
            MessagesStats.StatsMap.Clear();

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName = default) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData =>
            PrepareMessage().AliveForOneFrame().PostImmediate(
                entityManager,
                new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}