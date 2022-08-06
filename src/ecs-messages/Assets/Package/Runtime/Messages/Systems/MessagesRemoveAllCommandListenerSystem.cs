using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
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
            bool removeRequested = GetEntityQuery(ComponentType.ReadOnly<RemoveAllMessagesCommand>())
                .CalculateEntityCount() > 0;

            if (!removeRequested)
                return;
            
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityQuery allMessages = GetEntityQuery(ComponentType.ReadOnly<MessageTag>());

            ecb.DestroyEntitiesForEntityQuery(allMessages);
        }
    }
}