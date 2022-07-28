using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public struct MessageBuilder
    {
        internal MessageContext Context; 
        internal MessageLifetime Lifetime;
        internal float Milliseconds;
        
        internal EntityCommandBuffer Ecb;

        public MessageBuilder WithLifeTime(float milliseconds)
        {
            Lifetime = MessageLifetime.TimeRange;
            Milliseconds = milliseconds;

            return this;
        }
        
        public MessageBuilder WithUnlimitedLifeTime()
        {
            Lifetime = MessageLifetime.Unlimited;

            return this;
        }
    }
}