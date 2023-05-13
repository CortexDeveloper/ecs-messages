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

        /// <summary>
        /// Sets lifetime to OneFrame type.
        /// </summary>
        /// <returns>Special struct which stores message settings and provides API to build calls chain.</returns>
        public MessageBuilder AliveForOneFrame()
        {
            Lifetime = MessageLifetime.OneFrame;

            return this;
        }
        
        /// <summary>
        /// Sets lifetime to TimeInterval type.
        /// </summary>
        /// <param name="seconds">Amount of message lifetime in seconds.</param>
        /// <returns>Special struct which stores message settings and provides API to build calls chain.</returns>
        public MessageBuilder AliveForSeconds(float seconds)
        {
            Lifetime = MessageLifetime.TimeInterval;
            LifetimeSeconds = seconds;

            return this;
        }
        
        public MessageBuilder AliveForUnlimitedTime()
        {
            Lifetime = MessageLifetime.Unlimited;

            return this;
        }
        
        /// <summary>
        /// Creates entity-message by scheduling it to ECB. Burst is supported.
        /// </summary>
        /// <param name="ecb">ECB from needed world.</param>
        /// <param name="component">Message content.</param>
        /// <typeparam name="T">Component must implement IMessageComponent interface.</typeparam>
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
            else if (Lifetime == MessageLifetime.TimeInterval)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeInterval { LifetimeLeft = LifetimeSeconds });
            else if (Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
            
            ecb.AddComponent(messageEntity, component);
        }
        
        /// <summary>
        /// Immediately creates entity-message. Burst not supported.
        /// </summary>
        /// <param name="entityManager">EntityManger from needed world.</param>
        /// <param name="component">Message content.</param>
        /// <typeparam name="T">Component must implement IMessageComponent interface.</typeparam>
        /// <returns>Message entity.</returns>
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
            else if (Lifetime == MessageLifetime.TimeInterval)
                ecb.AddComponent(messageEntity, new MessageLifetimeTimeInterval { LifetimeLeft = LifetimeSeconds });
            else if (Lifetime == MessageLifetime.Unlimited)
                ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());

            ecb.AddComponent(messageEntity, component);
            
            ecb.Playback(entityManager);
            ecb.Dispose();

            return messageEntity;
        }
    }
}