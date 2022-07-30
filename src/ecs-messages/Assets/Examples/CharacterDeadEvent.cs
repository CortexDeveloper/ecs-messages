using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public struct CharacterDeadEvent : IComponentData
    {
        public int Tick;
    }
}