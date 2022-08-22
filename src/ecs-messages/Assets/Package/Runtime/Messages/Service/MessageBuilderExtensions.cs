using System;
using System.Diagnostics;
using CortexDeveloper.Messages.Components.Meta;
using Unity.Collections;
using Unity.Entities;
using Random = UnityEngine.Random;

namespace CortexDeveloper.Messages.Service
{
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

            Entity messageEntity = ecb.CreateEntity();
            Entity contentTargetEntity = builder.Entity != Entity.Null 
                ? builder.Entity
                : messageEntity;

            AddMetaComponents<T>(builder, messageEntity, ecb);
            
            ecb.AddComponent(contentTargetEntity, component);
        }

        public static void PostBuffer<T>(this MessageBuilder builder, params T[] elements) where T : struct, IBufferElementData
        {
            if (UniqueContentAlreadyExist<T>(builder) || UniqueAlreadyRequestedAtThisFrame<T>(builder))
                return;

            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();

            Entity messageEntity = ecb.CreateEntity();
            Entity contentTargetEntity = builder.Entity == Entity.Null 
                ? messageEntity 
                : builder.Entity;

            AddMetaComponents<T>(builder, messageEntity, ecb);

            DynamicBuffer<T> buffer = ecb.AddBuffer<T>(contentTargetEntity);

            for (int i = 0; i < elements.Length; i++)
                buffer.Add(elements[i]);
        }

        private static void AddMetaComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            if (builder.IsUnique)
                ecb.AddComponent<UniqueMessageTag>(messageEntity);

            AddEditorInfoComponents(messageEntity, ecb);
            AddContextComponents<T>(builder, messageEntity, ecb);
            AddLifetimeComponents(builder, messageEntity, ecb);
        }

        private static void AddContextComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(messageEntity, new MessageTag());

            if (builder.Entity != Entity.Null)
                ecb.AddComponent(messageEntity, new AttachedMessageContent
                {
                    ComponentType = new ComponentType(typeof(T)),
                    TargetEntity = builder.Entity
                });

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
        
        [Conditional("UNITY_EDITOR")]
        private static void AddEditorInfoComponents(Entity messageEntity, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(messageEntity, new MessageEditorData
            {
                Id = Random.Range(0, 99999999),
                CreationTime = DateTime.Now.ToString("HH:mm:ss")
            });
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

            return alreadyExist;
        }

        private static bool UniqueAlreadyRequestedAtThisFrame<T>(MessageBuilder builder)
        {
            if (!builder.IsUnique)
                return false;
            
            if (MessageBroadcaster.PostRequests.Contains(new ComponentType(typeof(T))))
                return true;

            MessageBroadcaster.PostRequests.Add(new ComponentType(typeof(T)));

            return false;
        }

        
    }
}