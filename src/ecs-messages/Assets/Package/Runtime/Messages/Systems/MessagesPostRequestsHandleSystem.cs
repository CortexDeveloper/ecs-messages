using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class MessagesPostRequestsHandleSystem : SystemBase
    {
        protected override void OnUpdate() => 
            MessageBroadcaster.ClearRequests();
    }
}