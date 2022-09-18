using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public class MessageBroadcasterSettings
    {
        public static readonly EntityManager EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
}