using System;
using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestAvailabilityData: IComponentData, IMessageComponent
    {
        public Quests Quest;
    }
}