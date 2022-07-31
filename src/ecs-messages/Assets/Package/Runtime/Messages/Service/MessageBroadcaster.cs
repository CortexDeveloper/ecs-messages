using CortexDeveloper.Messages.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        public static MessageBuilder PrepareEvent() => 
            new() {Context = MessageContext.Event};

        public static MessageBuilder PrepareCommand() => 
            new() {Context = MessageContext.Command};

        public static void RemoveAll() => 
            PrepareCommand().Post(new RemoveAllMessagesCommand());

        public static void RemoveWithLifetime(MessageLifetime lifetime)
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
            PrepareCommand().Post(new RemoveMessagesByComponentCommand{ ComponentType = new ComponentType(typeof(T)) });

        public static void RemoveBuffer<T>() where T : struct, IBufferElementData => 
            PrepareCommand().Post(new RemoveMessagesByComponentCommand{ ComponentType = new ComponentType(typeof(T)) });
    }

    public static class MessageBuilderExtensions
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem => 
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        public static void Post<T>(this MessageBuilder builder, T component) where T : struct, IComponentData
        {
            if (UniqueAttachmentAlreadyExist<T>(builder))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Cannot post unique message {typeof(T)}. Active instance already exist.");
#endif
                return;
            }

            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            Entity messageEntity = ecb.CreateEntity();

            AddInternalComponents(builder, messageEntity, ecb);

            ecb.AddComponent(messageEntity, component);
        }

        public static void PostBuffer<T>(this MessageBuilder builder, params T[] elements) where T : struct, IBufferElementData
        {
            if (UniqueAttachmentAlreadyExist<T>(builder))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Cannot post unique message {typeof(T)}. Active instance already exist.");
#endif
                return;
            }
            
            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            Entity messageEntity = ecb.CreateEntity();

            AddInternalComponents(builder, messageEntity, ecb);
            
            DynamicBuffer<T> buffer = ecb.AddBuffer<T>(messageEntity);
            
            for (int i = 0; i < elements.Length; i++) 
                buffer.Add(elements[i]);
        }

        private static bool UniqueAttachmentAlreadyExist<T>(MessageBuilder builder) where T : struct
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
            
            return alreadyExist;
        }

        private static void AddInternalComponents(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            if (builder.IsUnique)
                ecb.AddComponent<MessageUniqueTag>(messageEntity);

            AddContextComponents(builder, messageEntity, ecb);
            AddLifetimeComponents(builder, messageEntity, ecb);
        }

        private static void AddContextComponents(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(messageEntity, new MessageTag());
            
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
    }
}