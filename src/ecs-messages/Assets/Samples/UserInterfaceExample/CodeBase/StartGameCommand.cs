using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace Samples.UserInterfaceExample.CodeBase
{
    public struct StartGameCommand : IComponentData, IMessageComponent
    {
        public int Enemies;
        public float Duration;
    }
}