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

        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, EntityCommandBufferSystem ecbSystem, uint randomSeed = 1)
        {
            if (RandomGen.Data.state == 0)
                RandomGen.Data.InitState(randomSeed);

            MessagesSystemGroup messagesSystemGroup = world.CreateSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.AddSystemToUpdateList(messagesSystemGroup);

#if UNITY_EDITOR
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesStatsSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesDateTimeSystem>());
            
            MessagesStats.StatsMap.Add(world.Name, new Stats());
#endif

            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesOneFrameLifetimeSystem>().Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesTimeRangeLifetimeRemoveSystem>().Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesTimeRangeLifetimeTimerSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesRemoveByComponentCommandListenerSystem>().Construct(ecbSystem));
        }

        public static void Dispose(World world)
        {
#if UNITY_EDITOR
            MessagesStats.StatsMap.Remove(world.Name);
#endif

            world.DestroySystemManaged(world.GetExistingSystemManaged<MessagesSystemGroup>());
        }

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName = default) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData => 
            PrepareMessage().AliveForOneFrame().PostImmediate(entityManager, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}