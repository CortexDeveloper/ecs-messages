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
        [UnityTest]
        public IEnumerator PostOneSecondEvent_WaitFrame_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().AliveForTime(1f).Post(new TestContentData{ Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                                   component.Value == 123;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostTwoSecondsCommand_WaitOneSecond_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForTime(2f).Post(new TestContentData{ Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestsUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForTime(1f).Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }

        [UnityTest]
        public IEnumerator PostUniqueEvent_WaitFrame_PostUniqueEvent_CheckOnlyOneExist_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().AsUnique().AliveForTime(2f).Post(new TestContentData { Value = 123 });

            yield return null;
            
            MessageBroadcaster.PrepareEvent().AsUnique().AliveForTime(2f).Post(new TestContentData { Value = 123 });

            yield return null;
            
            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestsUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
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
            MessageBroadcaster.PrepareEvent().AsUnique().AttachedTo(firstEntity).AliveForTime(2f).Post(
                new TestContentData { Value = 123 });
            
            MessageBroadcaster.PrepareEvent().AsUnique().AttachedTo(secondEntity).AliveForTime(2f).Post(
                new TestContentData { Value = 123 });

            yield return null;
            
            // Assert
            EntityQuery query = TestsUtils.GetQuery<MessageTag>();
            EntityQuery attachedQuery = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(attachedQuery);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             attachedQuery.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             TestsUtils.FirstEntityHasComponent<AttachedMessageContent>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestsUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            entityManager.DestroyEntity(firstEntity);
            entityManager.DestroyEntity(secondEntity);
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
    }
}