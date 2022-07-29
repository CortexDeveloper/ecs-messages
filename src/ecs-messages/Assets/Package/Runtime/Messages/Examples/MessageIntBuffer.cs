using Unity.Entities;

namespace CortexDeveloper.Messages
{
    public struct MessageIntBuffer: IBufferElementData
    {
        public MessageIntData Value;
    }
}