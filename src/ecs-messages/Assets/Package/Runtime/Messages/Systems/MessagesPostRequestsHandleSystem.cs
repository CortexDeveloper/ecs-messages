using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    [AlwaysUpdateSystem]
    public partial class MessagesPostRequestsHandleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            MessageBroadcaster.ClearRequests();
        }
    }
}