using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Components.Meta
{
    public struct MessageEditorData : IComponentData
    {
        public int Id;
        public FixedString32Bytes CreationTime;
    }
}