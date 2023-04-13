using System.Collections;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.ECSMessages.Tests
{
    public class UnlimitedMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());

        [UnityTest]
        public IEnumerator Post_AliveForUnlimitedTime_WaitOneFrame_CheckForExisting_ManuallyRemoveAllUnlimited_WaitOneFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForUnlimitedTime()
                .Post(TestUtils.GetEcbSystem().CreateCommandBuffer(), new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                                   component.Value == 123;

            MessageBroadcaster.RemoveAllMessagesWith<MessageLifetimeUnlimitedTag>(TestUtils.GetEcbSystem().EntityManager);
            
            yield return null;

            bool wasRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostImmediate_AliveForUnlimitedTime_CheckForExisting_ManuallyRemoveImmediate_CheckForRemove()
        {
            // Act
            Entity messageEntity = MessageBroadcaster
                .PrepareMessage()
                .AliveForUnlimitedTime()
                .PostImmediate(TestUtils.GetEcbSystem().EntityManager, new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             component.Value == 123;

            TestUtils.GetTestWorld().EntityManager.DestroyEntity(messageEntity);

            bool wasRemoved = !TestUtils.IsEntityWithComponentExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);

            yield return null;
        }
    }
}