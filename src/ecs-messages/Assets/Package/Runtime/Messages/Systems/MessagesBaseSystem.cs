using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    public abstract partial class MessagesBaseSystem : SystemBase
    {
        protected EntityCommandBufferSystem EcbSystem;

        public MessagesBaseSystem Construct(EntityCommandBufferSystem ecbSystem)
        { 
            EcbSystem = ecbSystem;

            return this;
        }
    }
}