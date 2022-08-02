using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.SystemGroups;
using Unity.Entities;

namespace CortexDeveloper.Messages.Editor
{
    [UpdateInGroup(typeof(MessagesSystemGroup))]
    [AlwaysUpdateSystem]
    public partial class MessagesBroadcasterStatsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityQuery allMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageTag)));
            EntityQuery allEventsQuery = GetEntityQuery(new ComponentType(typeof(MessageContextEventTag)));
            EntityQuery allCommandsQuery = GetEntityQuery(new ComponentType(typeof(MessageContextCommandTag)));
            EntityQuery allUniqueQuery = GetEntityQuery(new ComponentType(typeof(MessageUniqueTag)));
            
            EntityQuery allOneFrameMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeOneFrameTag)));
            EntityQuery allTimeRangeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeTimeRange)));
            EntityQuery allUnlimitedLifetimeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeUnlimitedTag)));

            MessagesStats.ActiveMessagesCount = allMessagesQuery.CalculateEntityCount();
            MessagesStats.ActiveEventsCount = allEventsQuery.CalculateEntityCount();
            MessagesStats.ActiveCommandsCount = allCommandsQuery.CalculateEntityCount();
            MessagesStats.ActiveUniqueCount = allUniqueQuery.CalculateEntityCount();
            
            MessagesStats.ActiveOneFrameMessagesCount = allOneFrameMessagesQuery.CalculateEntityCount();
            MessagesStats.ActiveTimeRangeMessagesCount = allTimeRangeMessagesQuery.CalculateEntityCount();
            MessagesStats.ActiveUnlimitedLifetimeMessagesCount = allUnlimitedLifetimeMessagesQuery.CalculateEntityCount();
        }
    }
}