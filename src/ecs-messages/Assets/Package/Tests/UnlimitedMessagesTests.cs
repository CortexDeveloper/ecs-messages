using System.Collections;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Tests.Components;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class UnlimitedMessagesTests
    {
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem =>
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            
            MessageBroadcaster.InitializeInWorld(World.DefaultGameObjectInjectionWorld);
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }
        
        [UnityTest]
        public IEnumerator PostEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent(EcbSystem.CreateCommandBuffer()).AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                                   component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(EcbSystem.CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(EcbSystem.CreateCommandBuffer()).AliveForUnlimitedTime().Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(EcbSystem.CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostAttachedEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Arrange
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;

            Entity entity = entityManager.CreateEntity();

            // Act
            MessageBroadcaster.PrepareEvent(EcbSystem.CreateCommandBuffer()).AttachedTo(entity).AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<MessageTag>();
            EntityQuery attachedQuery = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirstEntity<TestContentData>(attachedQuery);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             attachedQuery.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<AttachedMessageContent>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(EcbSystem.CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsEntityWithComponentExist<TestContentData>();

            entityManager.DestroyEntity(entity);
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
    }
}