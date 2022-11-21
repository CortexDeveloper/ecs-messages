using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
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
            EntityManager entityManager = EntityManager;
            
            Entities
                .ForEach((Entity entity, in MessageTag messageTag, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    MessageUtils.Destroy(entity, ecb, entityManager);
                })
                .Schedule();
            
            CompleteDependency();
        }
    }
}