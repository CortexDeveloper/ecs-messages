using System;
using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct PauseGameCommand : IComponentData, IMessageComponent { }
}