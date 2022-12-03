using System.Diagnostics;
using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Systems;
using Unity.Entities;
using Unity.Mathematics;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBuilderExtensions
    {
        private static uint _seed;
        private static uint Seed => _seed < uint.MaxValue ? ++_seed : _seed = 0;
        
        public static void Post<T>(this MessageBuilder builder, EntityCommandBuffer ecb, T component) where T : struct, IComponentData, IMessageComponent
        {
            Entity messageEntity = ecb.CreateEntity();
            Entity contentTargetEntity = builder.MessageEntity != Entity.Null 
                ? builder.MessageEntity
                : messageEntity;

            AddMetaComponents<T>(builder, messageEntity, ecb);
            
            ecb.AddComponent(contentTargetEntity, component);
        }

        private static void AddMetaComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            AddEditorInfoComponents(messageEntity, ecb);
            AddContextComponents<T>(builder, messageEntity, ecb);
            AddLifetimeComponents(builder, messageEntity, ecb);
        }

        private static void AddContextComponents<T>(MessageBuilder builder, Entity messageEntity, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(messageEntity, new MessageTag());

            if (builder.MessageEntity != Entity.Null)
                ecb.AddComponent(messageEntity, new AttachedMessageContent
                {
                    ComponentType = ComponentType.ReadOnly<T>(),
                    TargetEntity = builder.MessageEntity
                });
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
                Id = new Random(Seed).NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
        }
    }
}