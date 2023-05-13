using System;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Service
{
    public static class WorldsCollectionExtensions
    {
        /// <summary>
        /// Returns an instance of world with given name.
        /// </summary>
        /// <param name="worlds">List of worlds stored in special collection.</param>
        /// <param name="worldName">Name of needed world.</param>
        /// <returns>World instance.</returns>
        /// <exception cref="Exception">If there is no world with given name this method will throw an exception.</exception>
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