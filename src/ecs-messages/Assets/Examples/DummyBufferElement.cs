using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct DummyBufferElement : IBufferElementData
    {
        public int Value;
    }
}