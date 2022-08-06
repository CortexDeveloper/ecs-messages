using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public struct MessageBuilder
    {
        internal MessageContext Context; 
        internal MessageLifetime Lifetime;
        internal float Seconds;
        internal bool IsUnique;
        internal Entity Entity;
        
        public MessageBuilder WithOneFrameLifeTime()
        {
            Lifetime = MessageLifetime.OneFrame;

            return this;
        }
        
        public MessageBuilder WithLifeTime(float seconds)
        {
            Lifetime = MessageLifetime.TimeRange;
            Seconds = seconds;

            return this;
        }
        
        public MessageBuilder WithUnlimitedLifeTime()
        {
            Lifetime = MessageLifetime.Unlimited;

            return this;
        }

        public MessageBuilder AsUnique()
        {
            IsUnique = true;

            return this;
        }
        
        public MessageBuilder AttachedTo(Entity entity)
        {
            Entity = entity;

            return this;
        }
    }
}