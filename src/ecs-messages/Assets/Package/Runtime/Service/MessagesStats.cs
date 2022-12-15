using System.Collections.Generic;

namespace CortexDeveloper.Messages.Service
{
    public static class MessagesStats
    {
        public static Dictionary<string, Stats> StatsMap { get; } = new();

        public static bool Enabled;
    }
}