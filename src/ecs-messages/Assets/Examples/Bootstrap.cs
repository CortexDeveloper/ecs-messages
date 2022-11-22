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
            MessageBroadcaster.Initialize(GetWorldByName("Example World"));
        }

        private World GetWorldByName(string name)
        {
            World.NoAllocReadOnlyCollection<World> worlds = World.All;
            foreach (World world in worlds)
            {
                if (world.Name == name)
                    return world;
            }

            return null;
        }
    }
}