using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Examples
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            MessageBroadcaster.Initialize(World.DefaultGameObjectInjectionWorld);
            MessageBroadcaster.Initialize(GetWorldWithName("Example World"));
        }

        private World GetWorldWithName(string worldName)
        {
            World.NoAllocReadOnlyCollection<World> worlds = World.All;
            foreach (World world in worlds)
            {
                if (world.Name == worldName)
                    return world;
            }

            return null;
        }
    }
}