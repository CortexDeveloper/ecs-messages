using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [AlwaysUpdateSystem]
    public partial class MessagesCreationRequestsHandleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            MessageBroadcaster.ClearRequests();
        }
    }
}