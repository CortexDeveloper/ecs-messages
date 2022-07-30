using Unity.Entities;

namespace CortexDeveloper.Messages.Components
{
    internal struct RemoveMessagesByComponentCommand : IComponentData
    {
        public ComponentType ComponentType;
    }
}