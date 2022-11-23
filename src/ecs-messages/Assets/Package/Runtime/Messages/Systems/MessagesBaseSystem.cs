using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    public abstract partial class MessagesBaseSystem : SystemBase
    {
        protected EntityCommandBufferSystem EcbSystem;

        protected override void OnCreate()
        {
            EcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    }
}