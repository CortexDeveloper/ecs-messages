using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    public abstract partial class MessagesBaseSystem : SystemBase
    {
        internal EntityCommandBufferSystem EcbSystem;

        internal EntityCommandBuffer MessagesEcb =>
            EcbSystem.CreateCommandBuffer();

        // TODO remove after converting all lifetime systems to ISystem
        internal MessagesBaseSystem Construct(EntityCommandBufferSystem ecbSystem)
        {
            EcbSystem = ecbSystem;
            
            return this;
        }
    }
}