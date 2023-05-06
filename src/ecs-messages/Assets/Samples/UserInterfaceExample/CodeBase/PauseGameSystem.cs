using Unity.Entities;
using UnityEngine;

namespace Samples.UserInterfaceExample
{
    [DisableAutoCreation]
    public partial struct PauseGameSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PauseGameCommand>();       
        }
        
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("Game paused");
        }
    }
}