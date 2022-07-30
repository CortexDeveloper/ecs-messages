using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    public partial class MessagesOneFrameLifetimeSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageLifetimeOneFrameTag>()));
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities
                .ForEach((Entity entity, in MessageLifetimeOneFrameTag oneFrameTag) => 
                    ecb.DestroyEntity(entity))
                .Schedule();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}