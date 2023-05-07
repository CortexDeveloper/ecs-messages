using System.Collections;
using CortexDeveloper.ECSMessages.Components.Meta;
using CortexDeveloper.ECSMessages.Service;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace CortexDeveloper.ECSMessages.Tests
{
    public class RemoveAPIMessagesTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => 
            TestUtils.InitializeTestWorld();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            MessageBroadcaster.DisposeFromWorld(TestUtils.GetTestWorld());
        
        [UnityTest]
        public IEnumerator PostImmediately_CheckForImmediateCreation_ManuallyRemove()
        {
            // Arrange 
            EntityManager entityManager = TestUtils.GetTestWorld().EntityManager;
            
            // Act
            Entity message = MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .PostImmediate(entityManager, new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                     TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                     TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                     component.Value == 123;
           
            entityManager.DestroyEntity(message);
            bool wasManuallyRemoved = query.CalculateEntityCount() == 0;
            
            Assert.IsTrue(wasPosted && wasManuallyRemoved);

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator PostImmediately_CheckForImmediateCreation_RemoveWithServiceAPI_WaitOneFrame_CheckForRemoving()
        {
            // Arrange 
            EntityManager entityManager = TestUtils.GetTestWorld().EntityManager;
            
            // Act
            MessageBroadcaster
                .PrepareMessage()
                .AliveForOneFrame()
                .PostImmediate(entityManager, new TestContentData { Value = 123 });

            // Assert
            EntityQuery query = TestUtils.GetQuery<TestContentData>();
            TestContentData component = TestUtils.GetComponentFromFirstEntity<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             component.Value == 123;
           
            MessageBroadcaster.RemoveAllMessagesWith<TestContentData>(entityManager);
            
            yield return null;
            
            bool wasRemoved = query.CalculateEntityCount() == 0;

            Assert.IsTrue(wasPosted && wasRemoved);
        }
    }
}