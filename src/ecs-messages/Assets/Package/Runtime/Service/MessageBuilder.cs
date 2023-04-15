using CortexDeveloper.ECSMessages.Components;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Systems;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Service
{
    public struct MessageBuilder
    {
        internal FixedString64Bytes Name;
        internal MessageLifetime Lifetime;
        internal float LifetimeSeconds;

        public MessageBuilder AliveForOneFrame()
        {
            Lifetime = MessageLifetime.OneFrame;

            return this;
        }
        
        public MessageBuilder AliveForSeconds(float seconds)
        {
            Lifetime = MessageLifetime.TimeRange;
            LifetimeSeconds = seconds;

            return this;
        }
        
        public MessageBuilder AliveForUnlimitedTime()
        {
            Lifetime = MessageLifetime.Unlimited;

            return this;
        }
        
        public void Post<T>(EntityCommandBuffer ecb, T component) where T : unmanaged, IComponentData, IMessageComponent
        {
            Entity messageEntity = ecb.CreateEntity();

#if UNITY_EDITOR
            ecb.SetName(messageEntity, Name);
            ecb.AddComponent(messageEntity, new MessageEditorData
            {
                Id = MessageBroadcaster.RandomGen.Data.NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            ecb.AddComponent(messageEntity, new MessageTag());

            if (Lifetime == MessageLifetime.OneFrame)
                ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
            else if (Lifetime == MessageLifetime.TimeRange)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = LifetimeSeconds });
            else if (Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
            
            ecb.AddComponent(messageEntity, component);
        }
        
        public Entity PostImmediate<T>(EntityManager entityManager, T component) where T : unmanaged, IComponentData, IMessageComponent
        {
            Entity messageEntity = entityManager.CreateEntity();
            EntityCommandBuffer ecb = new(Allocator.Temp);
            
#if UNITY_EDITOR
            ecb.SetName(messageEntity, Name);
            ecb.AddComponent(messageEntity, new MessageEditorData
            {
                Id = MessageBroadcaster.RandomGen.Data.NextInt(0, int.MaxValue),
                CreationTime = MessagesDateTimeSystem.TimeAsString.Data
            });
#endif
            
            ecb.AddComponent(messageEntity, new MessageTag());
            
            if (Lifetime == MessageLifetime.OneFrame)
                ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
            else if (Lifetime == MessageLifetime.TimeRange)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = LifetimeSeconds });
            else if (Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());

            ecb.AddComponent(messageEntity, component);
            
            ecb.Playback(entityManager);
            ecb.Dispose();

            return messageEntity;
        }
    }
}