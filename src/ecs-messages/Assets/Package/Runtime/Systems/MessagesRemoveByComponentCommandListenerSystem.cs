using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Components.RemoveCommands;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [UpdateBefore(typeof(MessagesOneFrameLifetimeSystem))]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct MessagesRemoveByComponentCommandListenerSystem : ISystem
    {
        private EntityQuery _removeByComponentCommandQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _removeByComponentCommandQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<MessageTag>(),
                ComponentType.ReadOnly<RemoveMessagesByComponentCommand>());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            NativeArray<RemoveMessagesByComponentCommand> commands = 
                _removeByComponentCommandQuery.ToComponentDataArray<RemoveMessagesByComponentCommand>(Allocator.Temp);

            foreach (RemoveMessagesByComponentCommand command in commands)
            {
                EntityQuery destroyQuery = GetMessagesQueryWith(ref state, command.ComponentType); 
                NativeArray<Entity> messageEntities = destroyQuery.ToEntityArray(Allocator.Temp);

                foreach (Entity messageEntity in messageEntities)
                    ecb.DestroyEntity(messageEntity);

                messageEntities.Dispose();
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            commands.Dispose();
        }

        private EntityQuery GetMessagesQueryWith(ref SystemState state, ComponentType componentType) => 
            componentType.TypeIndex == ComponentType.ReadOnly<MessageTag>().TypeIndex 
                ? state.GetEntityQuery(ComponentType.ReadOnly<MessageTag>()) 
                : state.GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), componentType);
    }
}