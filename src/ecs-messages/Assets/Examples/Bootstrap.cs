using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Examples
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            MessageBroadcaster.InitializeInWorld(World.DefaultGameObjectInjectionWorld);
            MessageBroadcaster.InitializeInWorld(World.All.GetWorldWithName("Example World"));
        }
    }
}