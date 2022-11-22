using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class MessagesRemoveAllCommandListenerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<RemoveAllMessagesCommand>()));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityQuery allMessages = GetEntityQuery(ComponentType.ReadOnly<MessageTag>());
            NativeArray<Entity> messageEntities = allMessages.ToEntityArray(Allocator.Temp);
            EntityManager entityManager = EntityManager;

            foreach (Entity messageEntity in messageEntities)
                MessageUtils.Destroy(messageEntity, ecb, entityManager);
        }
    }
}