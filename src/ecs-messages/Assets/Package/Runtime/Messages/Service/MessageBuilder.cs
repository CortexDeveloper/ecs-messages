using Unity.Collections;

namespace CortexDeveloper.Messages.Service
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
    }
}