using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    public partial class MessagesStatsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!MessagesStats.Enabled)
                return;
            
            EntityQuery allMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageTag)));
            EntityQuery allOneFrameMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeOneFrameTag)));
            EntityQuery allTimeRangeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeTimeRange)));
            EntityQuery allUnlimitedLifetimeMessagesQuery = GetEntityQuery(new ComponentType(typeof(MessageLifetimeUnlimitedTag)));

            string worldName = World.Name;
            
            MessagesStats.StatsMap[worldName].ActiveMessagesCount = allMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveOneFrameMessagesCount = allOneFrameMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveTimeRangeMessagesCount = allTimeRangeMessagesQuery.CalculateEntityCount();
            MessagesStats.StatsMap[worldName].ActiveUnlimitedLifetimeMessagesCount = allUnlimitedLifetimeMessagesQuery.CalculateEntityCount();
        }
    }
}