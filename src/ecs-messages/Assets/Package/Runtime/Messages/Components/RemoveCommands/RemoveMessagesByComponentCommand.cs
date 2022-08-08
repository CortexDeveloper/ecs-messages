using Unity.Entities;

namespace CortexDeveloper.Messages.Components.RemoveCommands
{
    internal struct RemoveMessagesByComponentCommand : IComponentData
    {
        public ComponentType ComponentType;
    }
}