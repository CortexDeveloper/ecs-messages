using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Components.RemoveCommands;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateBefore(typeof(MessagesOneFrameLifetimeSystem))]
    [DisableAutoCreation]
    public partial class MessagesRemoveByComponentCommandListenerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityManager entityManager = EntityManager;
            EntityCommandBuffer ecb = World
                .GetExistingSystem<EndSimulationEntityCommandBufferSystem>()
                .CreateCommandBuffer();
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<RemoveMessagesByComponentCommand>());
            NativeArray<RemoveMessagesByComponentCommand> commands = query.ToComponentDataArray<RemoveMessagesByComponentCommand>(Allocator.Temp);

            foreach (RemoveMessagesByComponentCommand command in commands)
            {
                EntityQuery destroyQuery = GetMessagesQueryWith(command.ComponentType); 
                NativeArray<Entity> messageEntities = destroyQuery.ToEntityArray(Allocator.Temp);

                foreach (Entity messageEntity in messageEntities)
                    ecb.DestroyEntity(messageEntity);

                messageEntities.Dispose();
            }
            
            commands.Dispose();
        }

        private EntityQuery GetMessagesQueryWith(ComponentType componentType) => 
            componentType.TypeIndex == ComponentType.ReadOnly<MessageTag>().TypeIndex 
                ? GetEntityQuery(ComponentType.ReadOnly<MessageTag>()) 
                : GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), componentType);
    }
}