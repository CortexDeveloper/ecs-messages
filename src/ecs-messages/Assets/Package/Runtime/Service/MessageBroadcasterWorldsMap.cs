using System.Collections.Generic;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Service
{
    internal static class MessageBroadcasterWorldsMap
    {
        internal static readonly Dictionary<World, ComponentSystemGroup> InitializedWorldStates = new();
    }
}