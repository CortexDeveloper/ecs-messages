using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Burst;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = MessagesEcb;
            
            new DestroyOneFrameMessagesJob
            {
                Ecb = ecb
            }.Schedule();
            
            Dependency.Complete();
        }
    }
    
    [BurstCompile]
    internal partial struct DestroyOneFrameMessagesJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;

        public void Execute(Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag)
        {
            Ecb.DestroyEntity(entity);
        }
    }
}