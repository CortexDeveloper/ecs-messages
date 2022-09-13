using System.Diagnostics;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Systems;
using Unity.Collections;
using Unity.Entities;
using Random = UnityEngine.Random;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBuilderExtensions
    {
        public static void Post<T>(this MessageBuilder builder, T component) where T : struct, IComponentData
        {
            EntityCommandBuffer ecb = builder.Ecb;
            Entity messageEntity = ecb.CreateEntity();
            Entity contentTargetEntity = builder.Entity != Entity.Null 
                ? builder.Entity
                : messageEntity;

            AddMetaComponents<T>(builder, messageEntity, ecb);
            
            ecb.AddComponent(contentTargetEntity, component);
        }
        
        public static void PostUnique<T>(this MessageBuilder builder, EntityManager entityManager, T component) where T : struct, IComponentData
        {
            if (UniqueContentAlreadyExist<T>(builder, entityManager) /*|| UniqueAlreadyRequestedAtThisFrame<T>(builder)*/)
                return;

            Post(builder, component);
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
                    ComponentType = ComponentType.ReadOnly<T>(),
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
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
        }

        private static bool UniqueContentAlreadyExist<T>(MessageBuilder builder, EntityManager entityManager) where T : struct
        {
            EntityQueryDescBuilder descBuilder = new(Allocator.Temp);
            descBuilder.AddAny(ComponentType.ReadOnly<T>());
            descBuilder.FinalizeQuery();
            
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