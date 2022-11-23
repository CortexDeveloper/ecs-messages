using CortexDeveloper.Messages.Service;
using UnityEditor;

namespace CortexDeveloper.Messages.Editor.Disposer
{
    [InitializeOnLoad]
    public static class MessagePostRequestDisposer
    {
        static MessagePostRequestDisposer()
        {
            EditorApplication.playModeStateChanged += Dispose;
        }
        
        private static void Dispose(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
                return;

            MessageBroadcaster.DisposePostRequests();
        }
    }
}