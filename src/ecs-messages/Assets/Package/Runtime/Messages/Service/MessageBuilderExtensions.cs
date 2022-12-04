using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Systems;
using Unity.Entities;
using Unity.Mathematics;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBuilderExtensions
    {
        private static uint _seed = 1;
        private static uint Seed => _seed < uint.MaxValue ? _seed++ : _seed = 1;
        
        public static void Post<T>(this MessageBuilder builder, EntityCommandBuffer ecb, T component) where T : struct, IComponentData, IMessageComponent
        {
            Entity messageEntity = ecb.CreateEntity();
            Entity contentTargetEntity = builder.MessageEntity != Entity.Null 
                ? builder.MessageEntity
                : messageEntity;

#if UNITY_EDITOR
            ecb.AddComponent(messageEntity, new MessageEditorData
            {
                Id = new Random(Seed).NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            ecb.AddComponent(messageEntity, new MessageTag());

            if (builder.MessageEntity != Entity.Null)
                ecb.AddComponent(messageEntity, new AttachedMessageContent
                {
                    ComponentType = ComponentType.ReadOnly<T>(),
                    TargetEntity = builder.MessageEntity
                });

            if (builder.Lifetime == MessageLifetime.OneFrame)
                ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
            else if (builder.Lifetime == MessageLifetime.TimeRange)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.Seconds });
            else if (builder.Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
            
            ecb.AddComponent(contentTargetEntity, component);
        }
        
        public static void PostImmediate<T>(this MessageBuilder builder, EntityManager entityManager, T component) where T : struct, IComponentData, IMessageComponent
        {
            Entity messageEntity = entityManager.CreateEntity();
            Entity contentTargetEntity = builder.MessageEntity != Entity.Null 
                ? builder.MessageEntity
                : messageEntity;
            
#if UNITY_EDITOR
            entityManager.AddComponentData(messageEntity, new MessageEditorData
            {
                Id = new Random(Seed).NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            entityManager.AddComponentData(messageEntity, new MessageTag());

            if (builder.MessageEntity != Entity.Null)
                entityManager.AddComponentData(messageEntity, new AttachedMessageContent
                {
                    ComponentType = ComponentType.ReadOnly<T>(),
                    TargetEntity = builder.MessageEntity
                });

            if (builder.Lifetime == MessageLifetime.OneFrame)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeOneFrameTag());
            else if (builder.Lifetime == MessageLifetime.TimeRange)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.Seconds });
            else if (builder.Lifetime == MessageLifetime.Unlimited)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeUnlimitedTag());

            entityManager.AddComponentData(contentTargetEntity, component);
        }
    }
}