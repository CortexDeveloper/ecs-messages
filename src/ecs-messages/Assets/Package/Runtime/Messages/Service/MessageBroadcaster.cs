using System;
using System.Collections.Generic;
using CortexDeveloper.Messages.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem => 
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        public static MessageBuilder PrepareEvent() => 
            CreateMessageBuilder(MessageContext.Event);

        public static MessageBuilder PrepareCommand() => 
            CreateMessageBuilder(MessageContext.Command);

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
            PrepareCommand().Post(new RemoveMessagesByComponentCommand{ ComponentType = new ComponentType(typeof(DynamicBuffer<T>)) });

        private static MessageBuilder CreateMessageBuilder(MessageContext context)
        {
            MessageBuilder messageBuilder = new();
            
            messageBuilder.Ecb = EcbSystem.CreateCommandBuffer();
            messageBuilder.Context = context;
            
            return messageBuilder;
        }
    }

    public static class MessageBuilderExtension
    {
        public static void Post<T>(this MessageBuilder builder, T component) where T : struct, IComponentData
        {
            if (UniqueAttachmentAlreadyExist<T>(builder))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Cannot post unique message {typeof(T)}. Active instance already exist.");
#endif
                return;
            }

            Entity messageEntity = builder.Ecb.CreateEntity();

            AddInternalComponents(builder, messageEntity);

            builder.Ecb.AddComponent(messageEntity, component);
        }

        public static void PostBuffer<T>(this MessageBuilder builder, params T[] elements) where T : struct, IBufferElementData
        {
            // TODO something went wrong, dont work with DynamicBuffer. Fix this little shit :D
            if (UniqueAttachmentAlreadyExist<DynamicBuffer<T>>(builder))
                return;
            
            Entity messageEntity = builder.Ecb.CreateEntity();

            AddInternalComponents(builder, messageEntity);
            
            DynamicBuffer<T> buffer = builder.Ecb.AddBuffer<T>(messageEntity);
            
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

        private static void AddInternalComponents(MessageBuilder builder, Entity messageEntity)
        {
            if (builder.IsUnique)
                builder.Ecb.AddComponent<MessageUniqueTag>(messageEntity);

            AddContextComponents(builder, messageEntity);
            AddLifetimeComponents(builder, messageEntity);
        }

        private static void AddContextComponents(MessageBuilder builder, Entity messageEntity)
        {
            builder.Ecb.AddComponent(messageEntity, new MessageTag());
            
            switch (builder.Context)
            {
                case MessageContext.Event:
                    builder.Ecb.AddComponent(messageEntity, new MessageContextEventTag());
                    break;
                case MessageContext.Command:
                    builder.Ecb.AddComponent(messageEntity, new MessageContextCommandTag());
                    break;
            }
        }

        private static void AddLifetimeComponents(MessageBuilder builder, Entity messageEntity)
        {
            switch (builder.Lifetime)
            {
                case MessageLifetime.OneFrame:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
                    break;
                case MessageLifetime.TimeRange:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.Seconds });
                    break;
                case MessageLifetime.Unlimited:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
                    break;
            }
        }
    }
}