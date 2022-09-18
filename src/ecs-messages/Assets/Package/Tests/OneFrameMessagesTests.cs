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
        private static EndSimulationEntityCommandBufferSystem _ecbSystem;
        private static EndSimulationEntityCommandBufferSystem EcbSystem =>
            _ecbSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        [UnityTest]
        public IEnumerator PostEvent_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent(EcbSystem.CreateCommandBuffer()).AliveForOneFrame().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                   component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostCommand_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand(EcbSystem.CreateCommandBuffer()).AliveForOneFrame().Post(new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }

        [UnityTest]
        public IEnumerator PostCommandAsUnique_PostCommandAsUnique_WaitFrame_CheckOnlyOneExist_WaitFrame_CheckForAutoRemove()
        {
            //Arrange 
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>()
                .EntityManager;
            
            // Act
            MessageBroadcaster.PrepareCommand(EcbSystem.CreateCommandBuffer()).AliveForOneFrame().PostUnique(entityManager, new TestContentData { Value = 123 });
            MessageBroadcaster.PrepareCommand(EcbSystem.CreateCommandBuffer()).AliveForOneFrame().PostUnique(entityManager, new TestContentData { Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
    }
}
