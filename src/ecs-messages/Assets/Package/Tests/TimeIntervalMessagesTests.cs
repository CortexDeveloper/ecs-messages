using System.Collections;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace CortexDeveloper.ECSMessages.Tests
{
    public class TimeIntervalMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());

        [UnityTest]
        public IEnumerator Post_AliveForOneSecond_WaitOneFrame_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForSeconds(1f)
                .Post(TestUtils.GetEcbSystem().CreateCommandBuffer(), new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeTimeInterval>(query) &&
                                   component.Value == 123;

            yield return new WaitForSeconds(1.01f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostImmediate_AliveForOneSecond_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForSeconds(1f)
                .PostImmediate(TestUtils.GetEcbSystem().EntityManager, new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeInterval>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1.01f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator Post_AliveForTwoSeconds_WaitOneSecond_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForSeconds(2f)
                .Post(TestUtils.GetEcbSystem().CreateCommandBuffer(), new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeTimeInterval>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1.01f);
            
            bool existedAfterOneSecondPassed = TestUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1.01f);

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
    }
}