using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public struct CharacterDeadEvent : IComponentData, IMessageComponent
    {
        public int Tick;
    }
}