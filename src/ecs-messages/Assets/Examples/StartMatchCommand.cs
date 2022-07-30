using System;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct StartMatchCommand : IComponentData
    {
        public Difficulty DifficultyLevel;
        public float MatchLength;
        public int EnemiesCount;
    }
}