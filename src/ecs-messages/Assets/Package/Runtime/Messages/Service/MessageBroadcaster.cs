using System.Collections.Generic;
using System.Diagnostics;
using CortexDeveloper.Messages.Components;
using Unity.Collections;
using Unity.Entities;
using Debug = UnityEngine.Debug;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        internal static readonly HashSet<ComponentType> PostRequests = new();

        public static MessageBuilder PrepareEvent() =>
            new() { Context = MessageContext.Event };

        public static MessageBuilder PrepareCommand() =>
            new() { Context = MessageContext.Command };

        public static void RemoveFrom(Entity entity)
        {
            
        }
        
        public static void RemoveAllCommon() =>
            PrepareCommand().Post(new RemoveAllMessagesCommand());
        
        public static void RemoveAllAttached() =>
            PrepareCommand().Post(new RemoveAllAttachedMessagesCommand());

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
                    Remove<MessageLifetimeOneFrameTag>();
                    break;
                case MessageLifetime.TimeRange:
                    Remove<MessageLifetimeTimeRange>();
                    break;
                case MessageLifetime.Unlimited:
                    Remove<MessageLifetimeUnlimitedTag>();
                    break;
            }
        }

        public static void Remove<T>() where T : struct, IComponentData =>
            PrepareCommand().Post(new RemoveMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveBuffer<T>() where T : struct, IBufferElementData =>
            PrepareCommand().Post(new RemoveMessagesByComponentCommand
                { ComponentType = new ComponentType(typeof(T)) });

        internal static void ClearRequests() =>
            PostRequests.Clear();
    }

    public static class MessageBuilderExtensions
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;

        private static EndSimulationEntityCommandBufferSystem EcbSystem =>
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        public static void Post<T>(this MessageBuilder builder, T component) where T : struct, IComponentData
        {
            if (UniqueContentAlreadyExist<T>(builder) || UniqueAlreadyRequestedAtThisFrame<T>(builder))
                return;

            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            Entity messageEntity = builder.Entity == Entity.Null
                ? ecb.CreateEntity()
                : builder.Entity;

            AddInternalComponents<T>(builder, messageEntity, ecb);

            ecb.AddComponent(messageEntity, component);
        }

        public static void PostBuffer<T>(this MessageBuilder builder, params T[] elements) where T : struct, IBufferElementData
        {
            if (UniqueContentAlreadyExist<T>(builder) || UniqueAlreadyRequestedAtThisFrame<T>(builder))
                return;

            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            Entity messageEntity = ecb.CreateEntity();

            AddInternalComponents<T>(builder, messageEntity, ecb);

            DynamicBuffer<T> buffer = ecb.AddBuffer<T>(messageEntity);

            for (int i = 0; i < elements.Length; i++)
                buffer.Add(elements[i]);
        }

        private static void AddInternalComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            if (builder.IsUnique)
                ecb.AddComponent<MessageUniqueTag>(messageEntity);

            AddContextComponents<T>(builder, messageEntity, ecb);
            AddLifetimeComponents(builder, messageEntity, ecb);
        }

        private static void AddContextComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            if (builder.Entity == Entity.Null)
                ecb.AddComponent(messageEntity, new MessageTag());
            else
                ecb.AddComponent(messageEntity, new AttachedMessage { ComponentType = new ComponentType(typeof(T))});

            switch (builder.Context)
            {
                case MessageContext.Event:
                    ecb.AddComponent(messageEntity, new MessageContextEventTag());
                    break;
                case MessageContext.Command:
                    ecb.AddComponent(messageEntity, new MessageContextCommandTag());
                    break;
            }
        }

        private static void AddLifetimeComponents(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            switch (builder.Lifetime)
            {
                case MessageLifetime.OneFrame:
                    ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
                    break;
                case MessageLifetime.TimeRange:
                    ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.Seconds });
                    break;
                case MessageLifetime.Unlimited:
                    ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
                    break;
            }
        }

        private static bool UniqueContentAlreadyExist<T>(MessageBuilder builder) where T : struct
        {
            if (!builder.IsUnique)
                return false;

            EntityQueryDescBuilder descBuilder = new(Allocator.Temp);
            descBuilder.AddAny(new ComponentType(typeof(T)));
            descBuilder.FinalizeQuery();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = entityManager.CreateEntityQuery(descBuilder);
            bool alreadyExist = query.CalculateEntityCount() > 0;

            descBuilder.Dispose();

            if (alreadyExist)
                LogWarning($"Cannot post unique message {typeof(T)}. Active instance already exist.");

            return alreadyExist;
        }

        private static bool UniqueAlreadyRequestedAtThisFrame<T>(MessageBuilder builder)
        {
            if (!builder.IsUnique)
                return false;
            
            if (MessageBroadcaster.PostRequests.Contains(new ComponentType(typeof(T))))
            {
                LogWarning($"Cannot post unique message {typeof(T)}. Message already requested at this frame.");

                return true;
            }

            MessageBroadcaster.PostRequests.Add(new ComponentType(typeof(T)));

            return false;
        }

        [Conditional("UNITY_EDITOR")]
        private static void LogWarning(string message)
        {
            if (MessageBroadcasterSettings.LogsEnabled)
                Debug.LogWarning(message);
        }
    }
}