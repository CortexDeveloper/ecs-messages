using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesOneFrameLifetimeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<MessageLifetimeOneFrameTag>()));
        }
        
        protected override void OnUpdate()
        {
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<MessageLifetimeOneFrameTag>());
            NativeArray<Entity> messageEntities = query.ToEntityArray(Allocator.Temp);
            EntityManager entityManager = EntityManager;

            foreach (Entity messageEntity in messageEntities)
                MessageUtils.DestroyImmediate(messageEntity, entityManager);
        }
    }
}