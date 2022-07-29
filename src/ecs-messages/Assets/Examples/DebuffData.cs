using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct DebuffData : IBufferElementData
    {
        public Debuffs Value;
    }
}