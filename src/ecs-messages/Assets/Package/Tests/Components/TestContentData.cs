using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Tests.Components
{
    public struct TestContentData : IComponentData, IMessageComponent
    {
        public int Value;
    }
}