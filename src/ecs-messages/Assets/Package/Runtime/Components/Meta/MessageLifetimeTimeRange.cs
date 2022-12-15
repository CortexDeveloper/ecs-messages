using Unity.Entities;

namespace CortexDeveloper.Messages.Components.Meta
{
    public struct MessageLifetimeTimeRange : IComponentData
    {
        public float LifetimeLeft;
    }
}