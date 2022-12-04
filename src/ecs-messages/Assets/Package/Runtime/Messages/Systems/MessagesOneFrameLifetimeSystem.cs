using CortexDeveloper.Messages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = MessagesEcb;
            
            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
        }
    }
}