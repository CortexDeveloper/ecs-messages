using Unity.Entities;
using UnityEngine;

namespace Samples.SystemToSystemExample
{
    [DisableAutoCreation]
    public partial struct CountdownAnalyticsTrackingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CountdownEndAnalyticsEvent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            new SendCountdownAnalyticsJob().Schedule();
        }
    }
    
    public partial struct SendCountdownAnalyticsJob : IJobEntity
    {
        public void Execute(in CountdownEndAnalyticsEvent countDownEvent)
        {
            Debug.Log($"Send countdown analytics. Circles passed: {countDownEvent.CirclesPassed}");
        }
    }
}