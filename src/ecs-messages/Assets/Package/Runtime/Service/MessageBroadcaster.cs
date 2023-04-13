using System;
using System.Collections.Generic;
using CortexDeveloper.ECSMessages.Components.RemoveCommands;
using CortexDeveloper.ECSMessages.SystemGroups;
using CortexDeveloper.ECSMessages.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace CortexDeveloper.ECSMessages.Service
{
    public static class MessageBroadcaster
    {
        internal static readonly SharedStatic<Random> RandomGen = SharedStatic<Random>.GetOrCreate<RandomKey, Random>();

        private static readonly Dictionary<World, ComponentSystemGroup> InitializedWorldStates = new();

        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, EntityCommandBufferSystem ecbSystem, uint randomSeed = 1)
        {
            if (InitializedWorldStates.ContainsKey(world))
                throw new Exception($"World {world.Name} has already initialized.");
            
            InitializedWorldStates.Add(world, parentSystemGroup);
            
            if (RandomGen.Data.state == 0)
                RandomGen.Data.InitState(randomSeed);

            MessagesSystemGroup messagesSystemGroup = world.CreateSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.AddSystemToUpdateList(messagesSystemGroup);

            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesOneFrameLifetimeSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesTimeRangeLifetimeRemoveSystem>().Construct(ecbSystem));
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesTimeRangeLifetimeTimerSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesRemoveByComponentCommandListenerSystem>().Construct(ecbSystem));
            
#if UNITY_EDITOR
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystemManaged<MessagesStatsSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesDateTimeSystem>());
            
            MessagesStats.StatsMap.Add(world.Name, new Stats());
#endif
        }

        public static void DisposeFromWorld(World world)
        {
            if (!InitializedWorldStates.ContainsKey(world))
                throw new Exception($"World {world.Name} has not initialized yet.");
            
            ComponentSystemGroup parentSystemGroup = (ComponentSystemGroup)world.GetExistingSystemManaged(InitializedWorldStates[world].GetType());
            MessagesSystemGroup messagesSystemGroup = world.GetExistingSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.RemoveSystemFromUpdateList(messagesSystemGroup);
            world.DestroySystemManaged(messagesSystemGroup);
      
            InitializedWorldStates.Remove(world);

#if UNITY_EDITOR
            MessagesStats.StatsMap.Remove(world.Name);
#endif
        }

        public static MessageBuilder PrepareMessage() =>
            new();

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData => 
            PrepareMessage().AliveForOneFrame().PostImmediate(entityManager, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}