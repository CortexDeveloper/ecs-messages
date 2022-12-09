using System;
using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestAvailabilityData: IComponentData, IMessageComponent
    {
        public Quests Quest;
    }
}