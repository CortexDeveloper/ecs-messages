using System.Collections;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.ECSMessages.Tests
{
    public class OneFrameMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());

        [UnityTest]
        public IEnumerator Post_WaitOneFrame_CheckForExisting_WaitOneFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .Post(TestUtils.GetEcbSystem().CreateCommandBuffer(), new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                   component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostImmediate_CheckForExisting_WaitOneFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .PostImmediate(TestUtils.GetEcbSystem().EntityManager, new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
    }
}
