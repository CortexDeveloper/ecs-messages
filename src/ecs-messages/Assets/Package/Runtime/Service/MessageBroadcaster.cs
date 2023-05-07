using System;
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

        public static void InitializeInWorld(World world, ComponentSystemGroup parentSystemGroup, uint randomSeed = 1)
        {
            if (MessageBroadcasterWorldsMap.InitializedWorldStates.ContainsKey(world))
                throw new Exception($"World {world.Name} has already been initialized.");
            
            MessageBroadcasterWorldsMap.InitializedWorldStates.Add(world, parentSystemGroup);
            
            if (RandomGen.Data.state == 0)
                RandomGen.Data.InitState(randomSeed);

            MessagesSystemGroup messagesSystemGroup = world.CreateSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.AddSystemToUpdateList(messagesSystemGroup);

            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesOneFrameLifetimeRemoveSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesTimeIntervalLifetimeRemoveSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesTimeIntervalLifetimeTimerSystem>());
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesRemoveByComponentCommandListenerSystem>());
            
#if UNITY_EDITOR
            messagesSystemGroup.AddSystemToUpdateList(world.CreateSystem<MessagesDateTimeSystem>());
#endif
        }

        public static void DisposeFromWorld(World world)
        {
            if (!MessageBroadcasterWorldsMap.InitializedWorldStates.ContainsKey(world))
                throw new Exception($"World {world.Name} has not initialized yet.");
            
            ComponentSystemGroup parentSystemGroup = (ComponentSystemGroup)world.GetExistingSystemManaged(MessageBroadcasterWorldsMap.InitializedWorldStates[world].GetType());
            MessagesSystemGroup messagesSystemGroup = world.GetExistingSystemManaged<MessagesSystemGroup>();
            parentSystemGroup.RemoveSystemFromUpdateList(messagesSystemGroup);
            world.DestroySystemManaged(messagesSystemGroup);
      
            MessageBroadcasterWorldsMap.InitializedWorldStates.Remove(world);
        }

        public static MessageBuilder PrepareMessage() =>
            new();

        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName) => 
            new() { Name = messageEntityName };

        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData => 
            PrepareMessage().AliveForOneFrame().PostImmediate(entityManager, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}