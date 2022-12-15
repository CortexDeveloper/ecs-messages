using System;
using Unity.Entities;

namespace CortexDeveloper.Messages.Service
{
    public static class WorldsCollectionExtensions
    {
        public static World GetWorldWithName(this World.NoAllocReadOnlyCollection<World> worlds, string worldName)
        {
            foreach (World world in worlds)
            {
                if (world.Name == worldName)
                    return world;
            }

            throw new Exception($"World with name {worldName} was not found.");
        }
    }
}