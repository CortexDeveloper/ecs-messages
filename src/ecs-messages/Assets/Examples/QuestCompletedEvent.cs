using System;
using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestCompletedEvent: IComponentData, IMessageComponent
    {
        public Quests Value;
    }
}