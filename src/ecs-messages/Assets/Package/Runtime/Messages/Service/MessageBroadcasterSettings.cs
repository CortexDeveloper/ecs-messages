using UnityEngine;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessageBroadcasterSettings
    {
        private const string LogsEnabledKey = "ECS_MESSAGES_LOGS_ENABLED";
        
        internal static bool LogsEnabled 
            => PlayerPrefs.GetInt(LogsEnabledKey, 1) == 1;
    }
}