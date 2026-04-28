using ECS.Tag;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS
{
    public sealed class CreateSpaceViewsSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            EcsAppContext context = systems.GetShared<EcsAppContext>();

            EcsFilter filter = world
                .Filter<SpaceTag>()
                .Inc<Element>()
                .Inc<SpatialVolume>()
                .Inc<Geometry>()
                .End();

            EcsPool<Element> elementPool = world.GetPool<Element>();
            EcsPool<SpatialVolume> volumePool = world.GetPool<SpatialVolume>();
            EcsPool<Geometry> geometryPool = world.GetPool<Geometry>();

            foreach (int entity in filter)
            {
                ref Element element = ref elementPool.Get(entity);
                ref SpatialVolume volume = ref volumePool.Get(entity);
                ref Geometry geometry = ref geometryPool.Get(entity);

                Mesh mesh = EcsExtrusionMeshBuilder.Build(volume);
                if (mesh == null)
                {
                    Debug.LogError($"Failed to build space mesh for: {element.name}");
                    continue;
                }

                var go = new GameObject(element.name);
                if (context.ViewRoot != null)
                    go.transform.SetParent(context.ViewRoot, false);

                var meshFilter = go.AddComponent<MeshFilter>();
                var meshRenderer = go.AddComponent<MeshRenderer>();
                Material material = CreateSpaceMaterial(context.DefaultMaterial, entity);

                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterial = material;

                geometry.mesh = mesh;
                geometry.material = material;
                geometry.transform = go.transform;

                Debug.Log($"[ECS] Space view created: {element.name}");
            }
        }

        private static Material CreateSpaceMaterial(Material defaultMaterial, int seed)
        {
            Material material = defaultMaterial != null
                ? new Material(defaultMaterial)
                : new Material(Shader.Find("Standard"));

            Color color = Color.HSVToRGB((seed * 0.17f) % 1f, 0.35f, 0.95f);
            color.a = 1f;

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);

            return material;
        }
    }
}
