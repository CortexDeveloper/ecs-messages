using Unity.Entities;

namespace CortexDeveloper.Messages.Components
{
    internal struct RemoveAttachedMessagesByComponentCommand : IComponentData
    {
        public ComponentType ComponentType;
    }
}