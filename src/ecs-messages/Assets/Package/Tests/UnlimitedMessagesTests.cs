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
        public IEnumerator PostUnlimitedEvent_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
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
        public IEnumerator PostUnlimitedBufferCommand_CheckForExisting_ManuallyRemove_WaitTwoFrames_CheckForRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForUnlimitedTime().PostBuffer(
                new TestContentBufferData { Value = 123 },
                new TestContentBufferData { Value = 456 },
                new TestContentBufferData { Value = 789 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetBufferQuery<TestContentBufferData>();
            DynamicBuffer<TestContentBufferData> buffer = TestsUtils.GetBufferFromFirst<TestContentBufferData>(query);
            bool wasPosted = query.CalculateEntityCount() == 1 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeUnlimitedTag>(query) &&
                             buffer[0].Value == 123 &&
                             buffer[1].Value == 456 &&
                             buffer[2].Value == 789;

            MessageBroadcaster.RemoveCommonWithLifetime(MessageLifetime.Unlimited);
            
            yield return null;
            yield return null;

            bool wasRemoved = !TestsUtils.IsBufferExist<TestContentBufferData>();
            
            Assert.IsTrue(wasPosted && wasRemoved);
        }
    }
}