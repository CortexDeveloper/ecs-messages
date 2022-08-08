using Unity.Entities;

namespace CortexDeveloper.Messages.Components
{
    public struct AttachedMessage : IComponentData
    {
        public ComponentType ComponentType;
        public Entity TargetEntity;
    }
}