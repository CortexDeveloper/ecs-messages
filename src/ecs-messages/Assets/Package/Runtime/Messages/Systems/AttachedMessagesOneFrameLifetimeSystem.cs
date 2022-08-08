using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class AttachedMessagesOneFrameLifetimeSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<AttachedMessage>(), ComponentType.ReadOnly<MessageLifetimeOneFrameTag>()));
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, in AttachedMessage attachedMessage, in MessageLifetimeOneFrameTag oneFrameTag) =>
                {
                    ecb.RemoveComponent(attachedMessage.TargetEntity, attachedMessage.ComponentType);
                    ecb.DestroyEntity(entity);
                })
                .Run();
        }
    }
}