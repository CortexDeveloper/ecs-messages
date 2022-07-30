using Unity.Entities;

namespace CortexDeveloper.Messages.Components
{
    public struct MessageLifetimeTimeRange : IComponentData
    {
        public float LifetimeLeft;
    }
}