using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Components.RemoveCommands;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [UpdateBefore(typeof(MessagesOneFrameLifetimeSystem))]
    [DisableAutoCreation]
    public partial class MessagesRemoveByComponentCommandListenerSystem : MessagesBaseSystem
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = MessagesEcb;
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