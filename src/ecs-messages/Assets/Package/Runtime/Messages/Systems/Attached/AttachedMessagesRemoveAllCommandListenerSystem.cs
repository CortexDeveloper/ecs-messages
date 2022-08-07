using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class AttachedMessagesRemoveAllCommandListenerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<RemoveAllAttachedMessagesCommand>()));
        }

        protected override void OnUpdate()
        {
            bool removeRequested = GetEntityQuery(ComponentType.ReadOnly<RemoveAllAttachedMessagesCommand>())
                .CalculateEntityCount() > 0;

            if (!removeRequested)
                return;
            
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityQuery allMessages = GetEntityQuery(ComponentType.ReadOnly<AttachedMessage>());
            NativeArray<Entity> entities = allMessages.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                AttachedMessage attachedMessage = EntityManager.GetComponentData<AttachedMessage>(entity);
                ecb.RemoveComponent(entity, attachedMessage.ComponentType);
                
                MessageUtils.RemoveMessageMetaComponents(entity, ecb, EntityManager);
            }

            entities.Dispose();
        }
    }
}