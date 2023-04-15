using CortexDeveloper.ECSMessages.Components.Meta;
using Unity.Burst;
using Unity.Entities;

namespace CortexDeveloper.ECSMessages.Systems
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct MessagesTimeIntervalLifetimeTimerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            new UpdateLifetimeLeftJob
            {
                DeltaTime = deltaTime
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    internal partial struct UpdateLifetimeLeftJob : IJobEntity
    {
        public float DeltaTime;
        
        public void Execute(ref MessageLifetimeTimeInterval timeInterval, in MessageTag messageTag) => 
            timeInterval.LifetimeLeft -= DeltaTime;
    }
}