using Unity.Entities;

namespace CortexDeveloper.Messages.Components.RemoveCommands
{
    internal struct RemoveAttachedMessagesByComponentCommand : IComponentData
    {
        public ComponentType ComponentType;
    }
}