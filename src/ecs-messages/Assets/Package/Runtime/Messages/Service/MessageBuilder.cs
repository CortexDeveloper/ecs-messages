using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public struct MessageBuilder
    {
        internal MessageContext Context; 
        internal MessageLifetime Lifetime;
        internal float Seconds;
        internal bool IsUnique;
        internal Entity MessageEntity;
        internal EntityCommandBuffer Ecb;
        
        public MessageBuilder AliveForOneFrame()
        {
            Lifetime = MessageLifetime.OneFrame;

            return this;
        }
        
        public MessageBuilder AliveForTime(float seconds)
        {
            Lifetime = MessageLifetime.TimeRange;
            Seconds = seconds;

            return this;
        }
        
        public MessageBuilder AliveForUnlimitedTime()
        {
            Lifetime = MessageLifetime.Unlimited;

            return this;
        }

        public MessageBuilder AttachedTo(Entity entity)
        {
            MessageEntity = entity;

            return this;
        }
    }
}