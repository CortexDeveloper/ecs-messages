using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : MessagesBaseSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<MessageLifetimeOneFrameTag>()));
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            EntityManager entityManager = EntityManager;
            
            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    MessageUtils.Destroy(entity, ecb, entityManager);
                })
                .Schedule();
            
            CompleteDependency();
        }
    }
}