using System.Collections;
using System.Linq;
using CortexDeveloper.Messages.Components;
using CortexDeveloper.Messages.Service;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace CortexDeveloper.Tests
{
    public class OneFrameMessagesTests
    {
        [UnityTest]
        public IEnumerator Post_Command_Check_Existing_WaitFrame_Check_AutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareCommand().Post(new TestContentData{ Value = 1 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextCommandTag>(query) &&
                                   component.Value == 1;
            
            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
            
            Assert.IsTrue(wasPosted);
        }
        
        [UnityTest]
        public IEnumerator Post_Event_Check_Existing_WaitFrame_Check_AutoRemove()
        {
            // Act
            MessageBroadcaster.PrepareEvent().Post(new TestContentData{ Value = 2 });
            yield return null;

            // Assert
            EntityQuery query = TestsUtils.GetQuery<TestContentData>();
            TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
            bool wasPosted = query.CalculateEntityCount() > 0 &&
                                   TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
                                   component.Value == 2;

            yield return null;

            bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
            
            Assert.IsTrue(wasPosted && wasAutoRemoved);
        }
        
        // [UnityTest]
        // public IEnumerator Post_OneFrame_Event_Buffer_Then_Check_For_Existing_WaitFrame_Check_For_AutoRemove()
        // {
        //     // Act
        //     MessageBroadcaster.PrepareEvent().PostBuffer(
        //         new TestContentBufferData { Value = 123 },
        //         new TestContentBufferData { Value = 456 },
        //         new TestContentBufferData { Value = 789 });
        //     
        //     yield return null;
        //
        //     // Assert
        //     EntityQuery query = TestsUtils.GetQuery<>();
        //     TestContentData component = TestsUtils.GetComponentFromFirst<TestContentData>(query);
        //     bool wasPosted = query.CalculateEntityCount() > 0 &&
        //                      TestsUtils.FirstEntityHasComponent<MessageContextEventTag>(query) &&
        //                      component.Value == 2;
        //
        //     yield return null;
        //
        //     bool wasAutoRemoved = !TestsUtils.IsExist<TestContentData>();
        //     
        //     Assert.IsTrue(wasPosted && wasAutoRemoved);
        // }
    }
}
