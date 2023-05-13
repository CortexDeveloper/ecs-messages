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

        /// <summary>
        /// Call this method in your entry-point to initialize ECSMessages in chosen world.
        /// </summary>
        /// <param name="world">World where systems would be initialized.</param>
        /// <param name="parentSystemGroup">Parent system group for internal service system.</param>
        /// <param name="randomSeed">Seed for Unity.Mathematics.random.</param>
        /// <exception cref="Exception">Exception would be thrown if you're trying to initialize service in already initialized world.</exception>
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

        /// <summary>
        /// Disposes ECSMessages service in given world.
        /// </summary>
        /// <param name="world">World where systems would be initialized.</param>
        /// <exception cref="Exception">Exception would be thrown if you're trying to dispose service in world where it wasn't initialized.</exception>
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

        /// <summary>
        /// Start point for service calls chain.
        /// </summary>
        /// <returns>Special struct which stores message settings and provides API to build calls chain.</returns>
        public static MessageBuilder PrepareMessage() =>
            new();

        /// <summary>
        /// Start point for service calls chain.
        /// </summary>
        /// <param name="messageEntityName">Name for entity-message in editor.</param>
        /// <returns></returns>
        public static MessageBuilder PrepareMessage(FixedString64Bytes messageEntityName) => 
            new() { Name = messageEntityName };

        /// <summary>
        /// Removes all messages with T component.
        /// </summary>
        /// <param name="entityManager">EntityManager from needed world.</param>
        /// <typeparam name="T">Component must implement IMessageComponent interface.</typeparam>
        public static void RemoveAllMessagesWith<T>(EntityManager entityManager) where T : struct, IComponentData => 
            PrepareMessage().AliveForOneFrame().PostImmediate(entityManager, new RemoveMessagesByComponentCommand { ComponentType = new ComponentType(typeof(T)) });
    }
}