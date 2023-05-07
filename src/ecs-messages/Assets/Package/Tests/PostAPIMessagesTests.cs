using System.Collections;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.ECSMessages.Tests
{
    public class PostAPIMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());
        
        [UnityTest]
        public IEnumerator PostWithECB_CheckForDelayedCreation_WaitOneFrame_CheckForCreationAfterECBSystem()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .Post(TestUtils.GetEcbSystem().CreateCommandBuffer(), new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            bool wasPostedBeforeECB = query.CalculateEntityCount() == 1;

            yield return null;
            
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPostedAfterECB = query.CalculateEntityCount() == 1 &&
                                     TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                     TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                     component.Value == 123;

            yield return null;
            
            Assert.IsTrue(!wasPostedBeforeECB && wasPostedAfterECB);
        }
        
        [UnityTest]
        public IEnumerator PostImmediatelyWithEntityManager_CheckForImmediateCreation()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .PostImmediate(TestUtils.GetTestWorld().EntityManager, new TestContentData { Value = 999 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                  TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                  TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                  component.Value == 999;

            Assert.IsTrue(wasPosted);
            
            yield return null;
        }
    }
}