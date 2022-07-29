using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public struct MessageBuilder
    {
        internal MessageContext Context; 
        internal MessageLifetime Lifetime;
        internal float Seconds;
        
        internal EntityCommandBuffer Ecb;

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
    }
}