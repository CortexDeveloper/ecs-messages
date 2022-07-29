using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct QuestCompletedData: IComponentData
    {
        public Quests Value;
    }
}