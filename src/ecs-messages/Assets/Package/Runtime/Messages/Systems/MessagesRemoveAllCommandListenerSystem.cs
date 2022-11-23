using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesRemoveAllCommandListenerSystem : MessagesBaseSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<RemoveAllMessagesCommand>()));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
            EntityQuery allMessages = GetEntityQuery(ComponentType.ReadOnly<MessageTag>());
            NativeArray<Entity> messageEntities = allMessages.ToEntityArray(Allocator.Temp);
            EntityManager entityManager = EntityManager;

            foreach (Entity messageEntity in messageEntities)
                MessageUtils.Destroy(messageEntity, ecb, entityManager);
        }
    }
}