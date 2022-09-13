using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public partial class BurstableExampleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity, in PauseGameCommand command) =>
                {
                    MessageBroadcaster
                        .PrepareEvent()
                        .AliveForTime(10f)
                        .Post(new QuestAvailabilityData { Quest = Quests.KillDiablo });
                })
                .Schedule();
        }
    }
}