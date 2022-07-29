using CortexDeveloper.Messages.Components;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    public partial class MessagesRemoveByLifetimeCommandListenerSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<RemoveMessagesByComponentCommand>()));
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<RemoveMessagesByComponentCommand>());
            NativeArray<RemoveMessagesByComponentCommand> commands = query.ToComponentDataArray<RemoveMessagesByComponentCommand>(Allocator.Temp);
            
            foreach (RemoveMessagesByComponentCommand command in commands)
            {
                EntityQuery destroyQuery = GetEntityQuery(command.ComponentType);
                //TODO also add message tags to query becoause any entities(even no messages) will be deleted
                
                ecb.DestroyEntitiesForEntityQuery(destroyQuery);
            }
            
            commands.Dispose();
        }
    }
}