using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Messages.Systems
{
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class MessagesStatsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!MessagesStats.Enabled)
                return;
            
            EntityQuery allMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageTag)));
            EntityQuery allAttachedMessagesQuery = GetEntityQuery(new ComponentType(typeof(AttachedMessageContent)));
            EntityQuery allEventsQuery = GetEntityQuery(new ComponentType(typeof(MessageContextEventTag)));
            EntityQuery allCommandsQuery = GetEntityQuery(new ComponentType(typeof(MessageContextCommandTag)));
            
            EntityQuery allOneFrameMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeOneFrameTag)));
            EntityQuery allTimeRangeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeTimeRange)));
            EntityQuery allUnlimitedLifetimeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeUnlimitedTag)));

            string worldName = World.Name;
            
            MessagesStats.StatsMap[worldName].ActiveMessagesCount = allMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveAttachedMessagesCount = allAttachedMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveEventsCount = allEventsQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveCommandsCount = allCommandsQuery.CalculateEntityCount();
            
            MessagesStats.StatsMap[worldName].ActiveOneFrameMessagesCount = allOneFrameMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveTimeRangeMessagesCount = allTimeRangeMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveUnlimitedLifetimeMessagesCount = allUnlimitedLifetimeMessagesQuery.CalculateEntityCount();
        }
    }
}