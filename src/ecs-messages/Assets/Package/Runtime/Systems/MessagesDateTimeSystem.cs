using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    public partial struct MessagesDateTimeSystem : ISystem
    {
        public static readonly SharedStatic<FixedString32Bytes> TimeAsString =
            SharedStatic<FixedString32Bytes>.GetOrCreate<MessagesDateTimeSystem, FixedString32Bytes>();

        public void OnCreate(ref SystemState state) => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state) => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");
    }
}