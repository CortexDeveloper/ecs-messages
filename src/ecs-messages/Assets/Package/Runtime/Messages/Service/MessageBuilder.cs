namespace CortexDeveloper.Messages.Service
{
    public struct MessageBuilder
    {
        internal MessageLifetime Lifetime;
        internal float Seconds;

        public MessageBuilder AliveForOneFrame()
        {
            Lifetime = MessageLifetime.OneFrame;

            return this;
        }
        
        public MessageBuilder AliveForSeconds(float seconds)
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
    }
}