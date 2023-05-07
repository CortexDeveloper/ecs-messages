using CortexDeveloper.ECSMessages.Service;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Samples.SystemToSystemExample
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct CountdownSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            float startValue = 5;
            
            state.EntityManager.CreateSingleton(
                new CountdownData
                {
                    StartValue = startValue,
                    CurrentValue = startValue
                });
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            UpdateCountdownJob job = new()
            {
                ECB = ecb,
                DeltaTime = deltaTime
            };

            job.Schedule();
            state.Dependency.Complete();
            
            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }
    
    [BurstCompile]
    public partial struct UpdateCountdownJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;
        
        public void Execute(ref CountdownData countdown)
        {
            countdown.CurrentValue -= DeltaTime;
    
            if (countdown.CurrentValue <= 0f)
            {
                countdown.CurrentValue = countdown.StartValue;
                countdown.CirclesCount++;

                MessageBroadcaster
                    .PrepareMessage()
                    .AliveForOneFrame()
                    .Post(ECB, new CountdownEndAnalyticsEvent { CirclesPassed = countdown.CirclesCount });
            }
        }
    }
}