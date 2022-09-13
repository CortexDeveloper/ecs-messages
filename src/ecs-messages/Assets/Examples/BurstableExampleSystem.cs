using CortexDeveloper.Messages.Service;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    public partial class BurstableExampleSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
            EntityManager entityManager = EntityManager;
            
            Entities
                .ForEach((Entity entity, in PauseGameCommand command) =>
                {
                    ecb
                        .PrepareEvent()
                        .AliveForTime(10f)
                        .PostUnique(entityManager, new QuestAvailabilityData { Quest = Quests.KillDiablo });
                })
                .Run();
        }
    }
}