using CortexDeveloper.ECSMessages.Service;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;

namespace CortexDeveloper.ECSMessages.Tests
{
    internal static class TestUtils
    {
        private const string ECS_MESSAGES_TESTS_WORLD_KEY = "ECS_MESSAGES_TESTS_WORLD_KEY";
        
        internal static EntityQuery GetQuery<T>() where T : struct, IComponentData
        {
            NativeList<ComponentType> queryComponents = new(Allocator.Temp);
            queryComponents.Add(new ComponentType(typeof(T)));

            EntityQueryBuilder descBuilder = new(Allocator.Temp);
            EntityQuery query = descBuilder
                .WithAny(ref queryComponents)
                .Build(GetTestWorld().EntityManager);

            descBuilder.Dispose();
            queryComponents.Dispose();

            return query;
        }
        
        internal static T GetComponentFromFirstEntity<T>(EntityQuery query) where T : unmanaged, IComponentData
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
            GetTestWorld().GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();

        public static World GetTestWorld()
        {
            string worldName = EditorPrefs.GetString(ECS_MESSAGES_TESTS_WORLD_KEY, "Default World");

            return World.All.GetWorldWithName(worldName);
        }

        public static void InitializeTestWorld()
        {
            World testWorld = GetTestWorld();
            MessageBroadcaster.InitializeInWorld(testWorld, testWorld.GetExistingSystemManaged<SimulationSystemGroup>());
        }
    }
}