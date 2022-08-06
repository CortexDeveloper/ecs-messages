using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class MessagesOneFrameLifetimeSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<MessageTag>(), ComponentType.ReadOnly<MessageLifetimeOneFrameTag>()));
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    ecb.DestroyEntity(entity);
                })
                .Schedule();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}