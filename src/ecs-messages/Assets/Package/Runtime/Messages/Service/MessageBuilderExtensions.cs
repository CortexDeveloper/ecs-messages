using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Systems;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBuilderExtensions
    {
        public static void Post<T>(this MessageBuilder builder, EntityCommandBuffer ecb, T component) where T : struct, IComponentData, IMessageComponent
        {
            Entity messageEntity = ecb.CreateEntity();

#if UNITY_EDITOR
            ecb.SetName(messageEntity, builder.Name);
            ecb.AddComponent(messageEntity, new MessageEditorData
            {
                Id = MessageBroadcaster.RandomGen.Data.NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            ecb.AddComponent(messageEntity, new MessageTag());

            if (builder.Lifetime == MessageLifetime.OneFrame)
                ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
            else if (builder.Lifetime == MessageLifetime.TimeRange)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.LifetimeSeconds });
            else if (builder.Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
            
            ecb.AddComponent(messageEntity, component);
        }
        
        public static Entity PostImmediate<T>(this MessageBuilder builder, EntityManager entityManager, T component) where T : struct, IComponentData, IMessageComponent
        {
            Entity messageEntity = entityManager.CreateEntity();

#if UNITY_EDITOR
            entityManager.SetName(messageEntity, builder.Name);
            entityManager.AddComponentData(messageEntity, new MessageEditorData
            {
                Id = MessageBroadcaster.RandomGen.Data.NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            entityManager.AddComponentData(messageEntity, new MessageTag());
            
            if (builder.Lifetime == MessageLifetime.OneFrame)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeOneFrameTag());
            else if (builder.Lifetime == MessageLifetime.TimeRange)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.LifetimeSeconds });
            else if (builder.Lifetime == MessageLifetime.Unlimited)
                entityManager.AddComponentData(messageEntity, new MessageLifetimeUnlimitedTag());

            entityManager.AddComponentData(messageEntity, component);

            return messageEntity;
        }
    }
}