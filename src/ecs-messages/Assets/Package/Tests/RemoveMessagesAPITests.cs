// using System.Collections;
// using CortexDeveloper.Messages.Service;
// using CortexDeveloper.Tests.Components;
// using Unity.Entities;
// using UnityEngine.TestTools;
//
// namespace CortexDeveloper.Tests
// {
//     public class RemoveMessagesAPITests
//     {
//         private static EndSimulationEntityCommandBufferSystem _ecbSystem;
//         private static EndSimulationEntityCommandBufferSystem EcbSystem =>
//             _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//         
//         [UnitySetUp]
//         public IEnumerator SetUp()
//         {
//             yield return new EnterPlayMode();
//             
//             MessageBroadcaster.Initialize(World.DefaultGameObjectInjectionWorld);
//         }
//         
//         [UnityTearDown]
//         public IEnumerator TearDown()
//         {
//             yield return new ExitPlayMode();
//         }
//         
//         [UnityTest]
//         public IEnumerator PostUnlimitedTimeEvent_RemoveMessage()
//         {
//             // Arrange
//             EntityCommandBuffer ecb = EcbSystem.CreateCommandBuffer();
//             
//             // Act
//             ecb.PrepareEvent().AliveForUnlimitedTime().Post(new TestContentData { Value = 123 });
//
//             yield return null;
//
//             //ecb.RemoveMessage();
//             
//             // Assert
//         }
//     }
// }