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
        [UnityTest]
        public IEnumerator PostEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                                   component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForUnlimitedTime().Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostAttachedEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Arrange
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;

            Entity entity = entityManager.CreateEntity();

            // Act
            MessageBroadcaster.PrepareEvent().AttachedTo(entity).AliveForUnlimitedTime().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<MessageTag>();
            EntityQuery attachedQuery = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(attachedQuery);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             attachedQuery.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<AttachedMessageContent>(query) &&
                             component.Value == 123;

            MessageBroadcaster.RemoveCommonWithLifetime(MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsExist<TestContentData>();

            entityManager.DestroyEntity(entity);
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
    }
}