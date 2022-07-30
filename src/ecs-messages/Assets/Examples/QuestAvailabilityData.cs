using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestAvailabilityData: IComponentData
    {
        public Quests Quest;
    }
}