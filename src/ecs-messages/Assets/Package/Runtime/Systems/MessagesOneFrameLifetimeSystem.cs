using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer.ParallelWriter ecb = MessagesEcb.AsParallelWriter();
            
            Entities
                .ForEach((Entity entity, int entityInQueryIndex, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();
            
            Dependency.Complete();
        }
    }
}