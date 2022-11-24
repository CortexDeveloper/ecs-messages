using Unity.Collections;
using Unity.Entities;

namespace CortexDeveloper.Tests
{
    internal static class TestsUtils
    {
        internal static EntityQuery GetQuery<T>() where T : struct, IComponentData
        {
            EntityQueryDescBuilder descBuilder = new(Allocator.Temp);
            descBuilder.AddAny(new ComponentType(typeof(T)));
            descBuilder.FinalizeQuery();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = entityManager.CreateEntityQuery(descBuilder);

            descBuilder.Dispose();

            return query;
        }
        
        internal static T GetComponentFromFirstEntity<T>(EntityQuery query) where T : struct, IComponentData
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity entity = GetFirstEntity(query);
            
            return entityManager.HasComponent<T>(entity);
        }
    }
}