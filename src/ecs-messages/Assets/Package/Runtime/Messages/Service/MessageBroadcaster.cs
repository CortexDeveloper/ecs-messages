using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class MessageBroadcaster
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem => 
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        public static MessageBuilder PrepareEvent()
        {
            MessageBuilder messageBuilder = CreateMessageBuilder();
            messageBuilder.Context = MessageContext.Event;

            return messageBuilder;
        }
        
        public static MessageBuilder PrepareCommand()
        {
            MessageBuilder messageBuilder = CreateMessageBuilder();
            messageBuilder.Context = MessageContext.Command;

            return messageBuilder;
        }

        private static MessageBuilder CreateMessageBuilder()
        {
            MessageBuilder messageBuilder = new();
            messageBuilder.Ecb = EcbSystem.CreateCommandBuffer();
            
            return messageBuilder;
        }
    }

    public static class MessageBuilderExtension
    {
        public static void Post<T>(this MessageBuilder builder, T component) where T : struct, IComponentData
        {
            Entity messageEntity = builder.Ecb.CreateEntity();

            AddContextComponents(builder, messageEntity);
            AddLifetimeComponents(builder, messageEntity);
            
            builder.Ecb.AddComponent(messageEntity, component);
        }

        private static void AddContextComponents(MessageBuilder builder, Entity messageEntity)
        {
            switch (builder.Context)
            {
                case MessageContext.Event:
                    builder.Ecb.AddComponent(messageEntity, new MessageContextEventTag());
                    break;
                case MessageContext.Command:
                    builder.Ecb.AddComponent(messageEntity, new MessageContextCommandTag());
                    break;
            }
        }

        private static void AddLifetimeComponents(MessageBuilder builder, Entity messageEntity)
        {
            switch (builder.Lifetime)
            {
                case MessageLifetime.OneFrame:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeOneFrameTag());
                    break;
                case MessageLifetime.TimeRange:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeTimeRange { LifetimeLeft = builder.Milliseconds });
                    break;
                case MessageLifetime.Unlimited:
                    builder.Ecb.AddComponent(messageEntity, new MessageLifetimeUnlimitedTag());
                    break;
            }
        }
    }
}