using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Tests
{
    public struct TestContentData : IComponentData, IMessageComponent
    {
        public int Value;
    }
}