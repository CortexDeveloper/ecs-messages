using Unity.Entities;

namespace CortexDeveloper.Messages.Components.RemoveCommands
{
    internal struct RemoveMessagesByComponentCommand : IComponentData, IMessageComponent
    {
        public ComponentType ComponentType;
    }
}