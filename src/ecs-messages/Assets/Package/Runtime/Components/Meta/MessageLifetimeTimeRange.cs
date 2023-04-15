using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Components.Meta
{
    public struct MessageLifetimeTimeRange : IComponentData
    {
        public float LifetimeLeft;
    }
}