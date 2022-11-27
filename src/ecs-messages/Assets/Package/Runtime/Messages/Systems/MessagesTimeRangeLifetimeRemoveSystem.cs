using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesTimeRangeLifetimeRemoveSystem : MessagesBaseSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), typeof(MessageLifetimeTimeRange)));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            EntityManager entityManager = EntityManager;

            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeTimeRange timeRange) =>
                {
                    if (timeRange.LifetimeLeft <= 0f)
                        MessageUtils.Destroy(entity, ecb, entityManager);
                })
                .Schedule();
            
            CompleteDependency();
        }
    }
}