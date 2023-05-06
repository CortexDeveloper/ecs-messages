using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace Samples.UserInterfaceExample.CodeBase
{
    public struct StartGameCommand : IComponentData, IMessageComponent
    {
        public float Duration;
        public int Enemies;
    }
}