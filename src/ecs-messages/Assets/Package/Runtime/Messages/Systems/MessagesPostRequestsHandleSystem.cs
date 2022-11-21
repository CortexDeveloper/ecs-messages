using CortexDeveloper.Messages.Service;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    public partial class MessagesPostRequestsHandleSystem : SystemBase
    {
        protected override void OnUpdate() => 
            MessageBroadcaster.ClearRequests();
    }
}