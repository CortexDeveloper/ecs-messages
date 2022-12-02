using System.Collections;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Tests.Components;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class OneFrameMessagesTests
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
        public IEnumerator PostEvent_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForOneFrame().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                   component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(TestUtils.GetEcbSystem().CreateCommandBuffer()).AliveForOneFrame().Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
    }
}
