using Unity.Entities;

namespace CortexDeveloper.Messages.Components.RemoveCommands
{
    internal struct RemoveAttachedMessagesByComponentCommand : IComponentData, IMessageComponent
    {
        public ComponentType ComponentType;
    }
}