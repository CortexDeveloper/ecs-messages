using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public struct CharacterDeadEvent : IComponentData, IMessageComponent
    {
        public int Tick;
    }
}