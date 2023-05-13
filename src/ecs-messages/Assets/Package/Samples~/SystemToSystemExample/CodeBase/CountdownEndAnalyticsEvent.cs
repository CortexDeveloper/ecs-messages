using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace Samples.SystemToSystemExample
{
    public struct CountdownEndAnalyticsEvent : IComponentData, IMessageComponent
    {
        public int CirclesPassed;
    }
}