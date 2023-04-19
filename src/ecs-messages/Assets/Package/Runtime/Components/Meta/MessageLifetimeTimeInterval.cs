using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Components.Meta
{
    public struct MessageLifetimeTimeInterval : IComponentData
    {
        public float LifetimeLeft;
    }
}