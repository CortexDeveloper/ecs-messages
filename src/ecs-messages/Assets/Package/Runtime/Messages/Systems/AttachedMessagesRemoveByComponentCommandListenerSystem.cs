using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class AttachedMessagesRemoveByComponentCommandListenerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<RemoveAttachedMessagesByComponentCommand>()));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<RemoveAttachedMessagesByComponentCommand>());
            NativeArray<RemoveAttachedMessagesByComponentCommand> commands = query.ToComponentDataArray<RemoveAttachedMessagesByComponentCommand>(Allocator.Temp);
            
            foreach (RemoveAttachedMessagesByComponentCommand command in commands)
            {
                EntityQuery destroyQuery = GetEntityQuery(ComponentType.ReadOnly<AttachedMessage>(), command.ComponentType);
                NativeArray<Entity> entities = destroyQuery.ToEntityArray(Allocator.Temp);

                foreach (Entity entity in entities) 
                    MessageUtils.RemoveFrom(entity);

                entities.Dispose();
            }
            
            commands.Dispose();
        }
    }
}