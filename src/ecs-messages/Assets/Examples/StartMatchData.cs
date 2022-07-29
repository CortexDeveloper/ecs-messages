using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct StartMatchData : IComponentData
    {
        public Difficulty DifficultyLevel;
        public float MatchLength;
        public int EnemiesCount;
    }
}