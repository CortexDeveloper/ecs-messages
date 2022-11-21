using CortexDeveloper.Messages.Service;
using Unity.Entities;
using UnityEngine;

namespace CortexDeveloper.Examples
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            World exampleWorld = new("Example World");
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(exampleWorld);
            
            MessageBroadcaster.Initialize(World.DefaultGameObjectInjectionWorld);
            MessageBroadcaster.Initialize(exampleWorld);
        }
    }
}