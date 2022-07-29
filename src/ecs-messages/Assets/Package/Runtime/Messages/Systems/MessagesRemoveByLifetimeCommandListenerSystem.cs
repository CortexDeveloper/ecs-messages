using CortexDeveloper.Messages.Components;
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
            //TODO implement :D
        }
    }
}