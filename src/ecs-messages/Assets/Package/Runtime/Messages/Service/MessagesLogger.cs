using System.Diagnostics;

namespace CortexDeveloper.Messages.Service
{
    internal static class MessagesLogger
    {
        [Conditional("UNITY_EDITOR")]
        internal static void LogWarning(string message)
        {
            if (MessageBroadcasterSettings.LogsEnabled)
                UnityEngine.Debug.LogWarning(message);
        }
    }
}