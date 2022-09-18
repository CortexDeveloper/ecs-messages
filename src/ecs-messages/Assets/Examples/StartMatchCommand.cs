using System;
using CortexDeveloper.Messages.Components;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    [Serializable]
    public struct StartMatchCommand : IComponentData, IMessageComponent
    {
        public Difficulty DifficultyLevel;
        public float MatchLength;
        public int EnemiesCount;
    }
}