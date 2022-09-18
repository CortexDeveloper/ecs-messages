using System.Linq;
using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Examples
{
    // public partial class BurstableExampleSystem : SystemBase
    // {
    //     private EndSimulationEntityCommandBufferSystem _ecbSystem;
    //     private EntityQuery _myCachedQuery;
    //     private Entity _entity;
    //
    //     protected override void OnCreate()
    //     {
    //         _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    //         _entity = _myCachedQuery.ToEntityArray(Allocator.Temp).First();
    //     }
    //
    //     protected override void OnUpdate()
    //     {
    //         EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
    //         EntityManager entityManager = EntityManager;
    //         entityManager.AddComponent<DigGoldCommand>(_entity);
    //
    //         Entities
    //             .WithStoreEntityQueryInField(ref _myCachedQuery)
    //             .ForEach((Entity entity, in PauseGameCommand command) =>
    //             {
    //                 ecb
    //                     .PrepareEvent()
    //                     .AliveForTime(10f)
    //                     .PostUnique(entityManager, new QuestAvailabilityData { Quest = Quests.KillDiablo });
    //
    //                 StartMatchCommand startMatchCommand = GetComponent<StartMatchCommand>(entity);
    //                 Difficulty matchDifficulty = startMatchCommand.DifficultyLevel;
    //
    //                 DynamicBuffer<DummyBufferElement> dummyBuffer = GetBuffer<DummyBufferElement>(entity);
    //                 bool isBufferEmpty = dummyBuffer.IsEmpty;
    //             })
    //             .Run();
    //     }
    // }
}