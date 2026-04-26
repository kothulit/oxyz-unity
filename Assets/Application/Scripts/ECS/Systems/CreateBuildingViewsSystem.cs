using ECS.Tag;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS
{
    public sealed class CreateBuildingViewsSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var context = systems.GetShared<EcsAppContext>();

            var filter = world
                .Filter<BuildingTag>()
                .Inc<Element>()
                .Inc<ExtrusionGeometry>()
                .Inc<Geometry>()
                .End();

            var elementPool = world.GetPool<Element>();
            var extrusionPool = world.GetPool<ExtrusionGeometry>();
            var geometryPool = world.GetPool<Geometry>();

            foreach (int entity in filter)
            {
                ref Element element = ref elementPool.Get(entity);
                ref ExtrusionGeometry extrusion = ref extrusionPool.Get(entity);
                ref Geometry geometry = ref geometryPool.Get(entity);

                Mesh mesh = EcsExtrusionMeshBuilder.Build(extrusion);
                if (mesh == null)
                {
                    Debug.LogError($"Failed to build mesh for: {element.name}");
                    continue;
                }

                var go = new GameObject(element.name);
                if (context.ViewRoot != null)
                    go.transform.SetParent(context.ViewRoot, false);

                var meshFilter = go.AddComponent<MeshFilter>();
                var meshRenderer = go.AddComponent<MeshRenderer>();

                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterial = context.DefaultMaterial;

                geometry.mesh = mesh;
                geometry.material = context.DefaultMaterial;
                geometry.transform = go.transform;

                Debug.Log($"[ECS] Building view created: {element.name}");
            }
        }
    }
}