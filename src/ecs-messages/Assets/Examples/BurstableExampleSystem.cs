// using System.Linq;
// using CortexDeveloper.Messages.Service;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace CortexDeveloper.Examples
// {
//     public partial class BurstableExampleSystem : SystemBase
//     {
//         private EndSimulationEntityCommandBufferSystem _ecbSystem;
//     
//         protected override void OnCreate()
//         {
//             _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//         }
//     
//         protected override void OnUpdate()
//         {
//             EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();
//
//             Entities
//                 .ForEach((Entity entity, in PauseGameCommand command) =>
//                 {
//                     MessageBroadcaster
//                         .PrepareMessage()
//                         .AliveForSeconds(10f)
//                         .Post(ecb, new QuestAvailabilityData { Quest = Quests.SavePrincess });
//                 })
//                 .Run();
//         }
//     }
// }