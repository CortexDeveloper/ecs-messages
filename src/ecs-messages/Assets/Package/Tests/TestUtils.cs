using CortexDeveloper.Messages.Service;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;

namespace CortexDeveloper.Tests
{
    internal static class TestUtils
    {
        internal static EntityQuery GetQuery<T>() where T : struct, IComponentData
        {
            EntityQueryDescBuilder descBuilder = new(Allocator.Temp);
            descBuilder.AddAny(new ComponentType(typeof(T)));
            descBuilder.FinalizeQuery();

            EntityManager entityManager = GetTestWorld().EntityManager;
            EntityQuery query = entityManager.CreateEntityQuery(descBuilder);

            descBuilder.Dispose();

            return query;
        }
        
        internal static T GetComponentFromFirstEntity<T>(EntityQuery query) where T : struct, IComponentData
        {
            EntityManager entityManager = GetTestWorld().EntityManager;
            Entity entity = GetFirstEntity(query);
            T component = entityManager.GetComponentData<T>(entity);

            return component;
        }

        internal static Entity GetFirstEntity(EntityQuery query)
        {
            NativeArray<Entity> array = query.ToEntityArray(Allocator.Temp);
            Entity entity = array[0];
            
            array.Dispose();

            return entity;
        }
        
        internal static bool IsEntityWithComponentExist<T>() where T : struct, IComponentData => 
            GetQuery<T>().CalculateEntityCount() > 0;

        internal static bool FirstEntityHasComponent<T>(EntityQuery query) where T : struct, IComponentData
        {
            EntityManager entityManager = GetTestWorld().EntityManager;
            Entity entity = GetFirstEntity(query);
            
            return entityManager.HasComponent<T>(entity);
        }

        internal static EntityCommandBufferSystem GetEcbSystem() => 
            GetTestWorld().GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        public static World GetTestWorld()
        {
            string worldName = EditorPrefs.GetString(TestsConstants.TESTS_WORLD_KEY, "Default World");

            return World.All.GetWorldWithName(worldName);
        }

        public static void InitializeTestWorld()
        {
            World testWorld = GetTestWorld();
            MessageBroadcaster.InitializeInWorld(
                testWorld,
                testWorld.GetOrCreateSystem<SimulationSystemGroup>(),
                testWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>());
        }
    }
}