using Unity.Entities;

namespace Samples.SystemToSystemExample
{
    public struct CountdownData : IComponentData
    {
        public float StartValue;
        public float CurrentValue;
        public int CirclesCount;
    }
}