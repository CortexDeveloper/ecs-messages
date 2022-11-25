using System.Collections;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Tests.Components;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class TimeRangeMessagesTests
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
            yield return new ExitPlayMode();
        }
        
        [UnityTest]
        public IEnumerator PostOneSecondEvent_WaitFrame_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForSeconds(1f).Post(new TestContentData{ Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                                   component.Value == 123;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostTwoSecondsCommand_WaitOneSecond_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForSeconds(2f).Post(new TestContentData{ Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForSeconds(1f).Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }

        [UnityTest]
        public IEnumerator PostUniqueEvent_WaitFrame_PostUniqueEvent_CheckOnlyOneExist_CheckForAutoRemove()
        {
            // Arrange
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;
            
            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForSeconds(2f).PostUnique(entityManager, new TestContentData { Value = 123 });

            yield return null;
            
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForSeconds(2f).PostUnique(entityManager, new TestContentData { Value = 123 });

            yield return null;
            
            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostUniqueAttachedCommand_PostUniqueAttachedCommand_WaitFrame_CheckOnlyOneExist_CheckForAutoRemove()
        {
            // Arrange
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;

            Entity firstEntity = entityManager.CreateEntity();
            Entity secondEntity = entityManager.CreateEntity();
            
            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AttachedTo(firstEntity).AliveForSeconds(2f).PostUnique(
                entityManager,
                new TestContentData { Value = 123 });
            
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AttachedTo(secondEntity).AliveForSeconds(2f).PostUnique(
                entityManager,
                new TestContentData { Value = 123 });

            yield return null;
            
            // Assert
            EntityQuery query = TestUtils.GetQuery<MessageTag>();
            EntityQuery attachedQuery = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(attachedQuery);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             attachedQuery.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             TestUtils.FirstEntityHasComponent<AttachedMessageContent>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            entityManager.DestroyEntity(firstEntity);
            entityManager.DestroyEntity(secondEntity);
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
    }
}