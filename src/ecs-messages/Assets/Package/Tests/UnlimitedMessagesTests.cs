using System.Collections;
using CortexDeveloper.Messages.Components.Meta;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Tests.Components;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class UnlimitedMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());
        
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }
        
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
            NativeArray<Entity> queryEntities = query.ToEntityArray(Allocator.Temp);
            queryEntities.Dispose();
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