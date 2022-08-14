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
        [UnityTest]
        public IEnumerator PostEvent_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().AliveForOneFrame().Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                                   component.Value == 123;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostBufferCommand_WaitFrame_CheckForExisting_WaitFrame_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForOneFrame().PostBuffer(
                new TestContentBufferData { Value = 123 },
                new TestContentBufferData { Value = 456 },
                new TestContentBufferData { Value = 789 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetBufferQuery<TestContentBufferData>();
            DynamicBuffer<TestContentBufferData> buffer = TestsUtils.GetBufferFromFirst<TestContentBufferData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeOneFrameTag>(query) &&
                             buffer[0].Value == 123 &&
                             buffer[1].Value == 456 &&
                             buffer[2].Value == 789;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsBufferExist<TestContentBufferData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
    }
}
