using System;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class MessagesDateTimeSystem : SystemBase
    {
        public static readonly SharedStatic<FixedString32Bytes> TimeAsString =
            SharedStatic<FixedString32Bytes>.GetOrCreate<MessagesDateTimeSystem, FixedString32Bytes>();

        protected override void OnCreate() => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");

        protected override void OnUpdate() => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");
    }
}