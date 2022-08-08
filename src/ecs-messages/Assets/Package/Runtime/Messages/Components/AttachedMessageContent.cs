using Unity.Entities;

namespace CortexDeveloper.Messages.Components
{
    public struct AttachedMessageContent : IComponentData
    {
        public ComponentType ComponentType;
        public Entity TargetEntity;
    }
}