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
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            
            MessageBroadcaster.InitializeInWorld(TestUtils.GetTestWorld());
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            MessageBroadcaster.Dispose();

            yield return new ExitPlayMode();
        }
        
        [UnityTest]
        public IEnumerator PostEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                                   component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(TestUtils.GetEcbSystem().CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForUnlimitedTime().Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(TestUtils.GetEcbSystem().CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostAttachedEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Arrange
            EntityManager entityManager = TestUtils.GetTestWorld()
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;

            Entity entity = entityManager.CreateEntity();

            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AttachedTo(entity).AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<MessageTag>();
            EntityQuery attachedQuery = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(attachedQuery);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             attachedQuery.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             TestUtils.FirstEntityHasComponent<AttachedMessageContent>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(TestUtils.GetEcbSystem().CreateCommandBuffer(), MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();

            entityManager.DestroyEntity(entity);
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
    }
}