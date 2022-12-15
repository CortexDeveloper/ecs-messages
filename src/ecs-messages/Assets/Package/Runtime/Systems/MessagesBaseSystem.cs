using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    public abstract partial class MessagesBaseSystem : SystemBase
    {
        internal EntityCommandBufferSystem EcbSystem;

        internal EntityCommandBuffer MessagesEcb =>
            EcbSystem.CreateCommandBuffer();

        internal MessagesBaseSystem Construct(EntityCommandBufferSystem ecbSystem)
        {
            EcbSystem = ecbSystem;
            
            return this;
        }
    }
}