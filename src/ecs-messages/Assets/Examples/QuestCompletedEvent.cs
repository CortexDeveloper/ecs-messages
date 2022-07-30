using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestCompletedEvent: IComponentData
    {
        public Quests Value;
    }
}