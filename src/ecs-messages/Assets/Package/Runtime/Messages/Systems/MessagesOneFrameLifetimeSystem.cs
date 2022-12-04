using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = World
                .GetExistingSystem<EndSimulationEntityCommandBufferSystem>()
                .CreateCommandBuffer();
            
            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
        }
    }
}