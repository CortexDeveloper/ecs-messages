using Unity.Entities;

namespace CortexDeveloper.Messages.Components.Meta
{
    public struct AttachedMessageContent : IComponentData
    {
        public ComponentType ComponentType;
        public Entity TargetEntity;
    }
}