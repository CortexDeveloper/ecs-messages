using System;
using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct PauseGameCommand : IComponentData, IMessageComponent { }
}