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
        internal static readonly SharedStatic<Random> RandomGen = SharedStatic<Random>.GetOrCreate<RandomKey, Random>();

        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, EntityCommandBufferSystem ecbSystem)
        {
            if (RandomGen.Data.state == 0)
                RandomGen.Data.InitState(1);

            MessagesSystemGroup messagesSystemGroup = world.CreateSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.AddSystemToUpdateList(messagesSystemGroup);

            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesStatsSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesDateTimeSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesOneFrameLifetimeSystem>().Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesTimeRangeLifetimeRemoveSystem>().Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesTimeRangeLifetimeTimerSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesRemoveByComponentCommandListenerSystem>().Construct(ecbSystem));

            MessagesStats.StatsMap.Add(world.Name, new Stats());
        }

        public static void Dispose() => 
            MessagesStats.StatsMap.Clear();

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName = default) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData => 
            PrepareMessage().AliveForOneFrame().PostImmediate(entityManager, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}