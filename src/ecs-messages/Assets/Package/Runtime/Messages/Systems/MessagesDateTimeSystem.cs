using System;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class MessagesDateTimeSystem : SystemBase
    {
        public static readonly SharedStatic<FixedString32Bytes> TimeAsString = new();

        protected override void OnCreate() => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");

        protected override void OnUpdate() => 
            TimeAsString.Data = DateTime.Now.ToString("hh:mm:ss");
    }
}