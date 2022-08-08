using System.Collections;
using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using CortexDeveloper.Tests.Components;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class TimeRangeMessagesTests
    {
        [UnityTest]
        public IEnumerator PostOneSecondEvent_WaitFrame_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().AliveForTime(1f).Post(new TestContentData{ Value = 123 });
            
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                                   TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                                   component.Value == 123;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostTwoSecondsCommand_WaitOneSecond_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForTime(2f).Post(new TestContentData{ Value = 123 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                             TestsUtils.FirstEntityHasComponent<MessageTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             component.Value == 123;

            yield return new WaitForSeconds(1f);
            
            bool existedAfterOneSecondPassed = TestsUtils.GetQuery<TestContentData>().CalculateEntityCount() == 1;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && existedAfterOneSecondPassed && wasAutoRemoved);
        }
        
        [UnityTest]
        public IEnumerator PostBufferCommand_CheckForExisting_WaitOneSecond_CheckForAutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().AliveForTime(1f).PostBuffer(
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
                             TestsUtils.FirstEntityHasComponent<MessageLifetimeTimeRange>(query) &&
                             buffer[0].Value == 123 &&
                             buffer[1].Value == 456 &&
                             buffer[2].Value == 789;

            yield return new WaitForSeconds(1f);

            bool wasAutoRemoved = !TestsUtils.IsBufferExist<TestContentBufferData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
    }
}