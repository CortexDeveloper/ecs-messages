using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Components.RemoveCommands
{
    internal struct RemoveMessagesByComponentCommand : IComponentData, IMessageComponent
    {
        public ComponentType ComponentType;
    }
}