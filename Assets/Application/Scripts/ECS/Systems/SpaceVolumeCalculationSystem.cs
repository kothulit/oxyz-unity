using System.Collections.Generic;
using ECS.Tag;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS
{
    public sealed class SpaceVolumeCalculationSystem : IEcsInitSystem
    {
        private const float ElevationEpsilon = 0.001f;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            EcsFilter filter = world
                .Filter<SpaceTag>()
                .Inc<Element>()
                .Inc<HierarchyNode>()
                .Inc<SpacePlacement>()
                .End();

            EcsPool<Element> elementPool = world.GetPool<Element>();
            EcsPool<HierarchyNode> hierarchyPool = world.GetPool<HierarchyNode>();
            EcsPool<SpacePlacement> placementPool = world.GetPool<SpacePlacement>();
            EcsPool<SpatialDivision> divisionPool = world.GetPool<SpatialDivision>();
            EcsPool<SpatialVolume> volumePool = world.GetPool<SpatialVolume>();
            EcsPool<Geometry> geometryPool = world.GetPool<Geometry>();

            foreach (int entity in filter)
            {
                ref Element element = ref elementPool.Get(entity);
                ref HierarchyNode hierarchy = ref hierarchyPool.Get(entity);

                if (hierarchy.ParentEntity < 0 || !volumePool.Has(hierarchy.ParentEntity))
                {
                    Debug.LogWarning($"[ECS] Space skipped. Parent volume is not available: {element.name}");
                    continue;
                }

                ref SpatialVolume parentVolume = ref volumePool.Get(hierarchy.ParentEntity);
                if (!IsValidVolume(parentVolume))
                {
                    Debug.LogWarning($"[ECS] Space skipped. Parent volume is invalid: {element.name}");
                    continue;
                }

                ref SpacePlacement placement = ref placementPool.Get(entity);
                float insertZ = placement.insertPoint.z;
                float[] splitElevations = GetParentDividingPlaneElevations(hierarchy.ParentEntity, divisionPool);

                if (!TryResolveVerticalRange(parentVolume, splitElevations, insertZ, out float bottom, out float top))
                {
                    Debug.LogWarning($"[ECS] Space skipped. InsertPoint.z is outside parent volume: {element.name}");
                    continue;
                }

                ref SpatialVolume volume = ref AddOrGetVolume(entity, volumePool);
                volume.bottom = bottom;
                volume.top = top;
                volume.points = CopyPoints(parentVolume.points);

                if (!geometryPool.Has(entity))
                    geometryPool.Add(entity);
            }
        }

        private static bool IsValidVolume(SpatialVolume volume)
        {
            return volume.points != null
                   && volume.points.Length >= 3
                   && volume.top > volume.bottom;
        }

        private static float[] GetParentDividingPlaneElevations(
            int parentEntity,
            EcsPool<SpatialDivision> divisionPool)
        {
            if (!divisionPool.Has(parentEntity))
                return System.Array.Empty<float>();

            ref SpatialDivision division = ref divisionPool.Get(parentEntity);
            return division.dividingPlaneElevations ?? System.Array.Empty<float>();
        }

        private static bool TryResolveVerticalRange(
            SpatialVolume parentVolume,
            float[] splitElevations,
            float insertZ,
            out float bottom,
            out float top)
        {
            List<float> elevations = BuildElevationSequence(parentVolume, splitElevations);

            for (int i = 0; i < elevations.Count - 1; i++)
            {
                float segmentBottom = elevations[i];
                float segmentTop = elevations[i + 1];
                bool isLastSegment = i == elevations.Count - 2;

                if (insertZ >= segmentBottom - ElevationEpsilon
                    && (insertZ < segmentTop - ElevationEpsilon || isLastSegment && insertZ <= segmentTop + ElevationEpsilon))
                {
                    bottom = segmentBottom;
                    top = segmentTop;
                    return true;
                }
            }

            bottom = 0f;
            top = 0f;
            return false;
        }

        private static List<float> BuildElevationSequence(SpatialVolume parentVolume, float[] splitElevations)
        {
            var elevations = new List<float> { parentVolume.bottom };

            for (int i = 0; i < splitElevations.Length; i++)
            {
                float elevation = splitElevations[i];
                if (elevation <= parentVolume.bottom + ElevationEpsilon)
                    continue;
                if (elevation >= parentVolume.top - ElevationEpsilon)
                    continue;

                elevations.Add(elevation);
            }

            elevations.Add(parentVolume.top);
            elevations.Sort();

            for (int i = elevations.Count - 2; i >= 0; i--)
            {
                if (Mathf.Abs(elevations[i + 1] - elevations[i]) <= ElevationEpsilon)
                    elevations.RemoveAt(i + 1);
            }

            return elevations;
        }

        private static ref SpatialVolume AddOrGetVolume(int entity, EcsPool<SpatialVolume> volumePool)
        {
            if (volumePool.Has(entity))
                return ref volumePool.Get(entity);

            return ref volumePool.Add(entity);
        }

        private static Vector2[] CopyPoints(Vector2[] source)
        {
            var copy = new Vector2[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = source[i];
            }

            return copy;
        }
    }
}
