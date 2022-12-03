using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
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
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<MessageLifetimeTimeRange>());
            NativeArray<Entity> messageEntities = query.ToEntityArray(Allocator.Temp);
            EntityManager entityManager = EntityManager;

            foreach (Entity messageEntity in messageEntities)
            {
                MessageLifetimeTimeRange timeRange = entityManager.GetComponentData<MessageLifetimeTimeRange>(messageEntity);
                
                if (timeRange.LifetimeLeft <= 0)
                    MessageUtils.DestroyImmediate(messageEntity, entityManager);
            }
        }
    }
}