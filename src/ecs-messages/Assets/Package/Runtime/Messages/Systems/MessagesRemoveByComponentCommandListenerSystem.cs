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
    public partial class MessagesRemoveByComponentCommandListenerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<RemoveMessagesByComponentCommand>()));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityManager entityManager = EntityManager;
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<RemoveMessagesByComponentCommand>());
            NativeArray<RemoveMessagesByComponentCommand> commands = query.ToComponentDataArray<RemoveMessagesByComponentCommand>(Allocator.Temp);

            foreach (RemoveMessagesByComponentCommand command in commands)
            {
                EntityQuery destroyQuery = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), command.ComponentType);
                NativeArray<Entity> messageEntities = destroyQuery.ToEntityArray(Allocator.Temp);

                foreach (Entity messageEntity in messageEntities)
                    MessageUtils.Destroy(messageEntity, ecb, entityManager);

                messageEntities.Dispose();
            }
            
            commands.Dispose();
        }
    }
}