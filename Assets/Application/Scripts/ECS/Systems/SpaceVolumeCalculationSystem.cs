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
            EcsPool<SpatialBoundarySet> boundarySetPool = world.GetPool<SpatialBoundarySet>();
            EcsPool<SpatialVolume> volumePool = world.GetPool<SpatialVolume>();
            EcsPool<SpatialVolumeSet> volumeSetPool = world.GetPool<SpatialVolumeSet>();
            EcsPool<Geometry> geometryPool = world.GetPool<Geometry>();

            foreach (int entity in filter)
            {
                ref Element element = ref elementPool.Get(entity);
                ref HierarchyNode hierarchy = ref hierarchyPool.Get(entity);

                if (hierarchy.ParentEntity < 0 || !volumeSetPool.Has(hierarchy.ParentEntity))
                {
                    Debug.LogWarning($"[ECS] Space skipped. Parent volume is not available: {element.name}");
                    continue;
                }

                ref SpatialVolumeSet parentVolumeSet = ref volumeSetPool.Get(hierarchy.ParentEntity);
                if (!IsValidVolumeSet(parentVolumeSet))
                {
                    Debug.LogWarning($"[ECS] Space skipped. Parent volume is invalid: {element.name}");
                    continue;
                }

                ref SpacePlacement placement = ref placementPool.Get(entity);
                Vector2 insertPlanPoint = new Vector2(placement.insertPoint.x, placement.insertPoint.y);
                float insertZ = placement.insertPoint.z;
                float[] splitElevations = GetParentDividingPlaneElevations(hierarchy.ParentEntity, divisionPool);
                SpatialBoundary[] boundaries = GetParentDividingBoundaries(hierarchy.ParentEntity, boundarySetPool);

                if (!TryResolveVerticalRange(parentVolumeSet, splitElevations, insertZ, out float bottom, out float top))
                {
                    Debug.LogWarning($"[ECS] Space skipped. InsertPoint.z is outside parent volume: {element.name}");
                    continue;
                }

                SpatialVolume[] clippedParts = ClipVolumeSet(parentVolumeSet, boundaries, insertPlanPoint, bottom, top);
                if (clippedParts.Length == 0)
                {
                    Debug.LogWarning($"[ECS] Space skipped. No parent volume parts match plan region and height range: {element.name}");
                    continue;
                }

                ref SpatialVolumeSet volumeSet = ref AddOrGetVolumeSet(entity, volumeSetPool);
                volumeSet.parts = clippedParts;

                ref SpatialVolume volume = ref AddOrGetVolume(entity, volumePool);
                volume = clippedParts[0];

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

        private static bool IsValidVolumeSet(SpatialVolumeSet volumeSet)
        {
            if (volumeSet.parts == null || volumeSet.parts.Length == 0)
                return false;

            for (int i = 0; i < volumeSet.parts.Length; i++)
            {
                if (IsValidVolume(volumeSet.parts[i]))
                    return true;
            }

            return false;
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

        private static SpatialBoundary[] GetParentDividingBoundaries(
            int parentEntity,
            EcsPool<SpatialBoundarySet> boundarySetPool)
        {
            if (!boundarySetPool.Has(parentEntity))
                return System.Array.Empty<SpatialBoundary>();

            ref SpatialBoundarySet boundarySet = ref boundarySetPool.Get(parentEntity);
            return boundarySet.boundaries ?? System.Array.Empty<SpatialBoundary>();
        }

        private static bool TryResolveVerticalRange(
            SpatialVolumeSet parentVolumeSet,
            float[] splitElevations,
            float insertZ,
            out float bottom,
            out float top)
        {
            GetVerticalBounds(parentVolumeSet, out float parentBottom, out float parentTop);
            List<float> elevations = BuildElevationSequence(parentBottom, parentTop, splitElevations);

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

        private static List<float> BuildElevationSequence(float parentBottom, float parentTop, float[] splitElevations)
        {
            var elevations = new List<float> { parentBottom };

            for (int i = 0; i < splitElevations.Length; i++)
            {
                float elevation = splitElevations[i];
                if (elevation <= parentBottom + ElevationEpsilon)
                    continue;
                if (elevation >= parentTop - ElevationEpsilon)
                    continue;

                elevations.Add(elevation);
            }

            elevations.Add(parentTop);
            elevations.Sort();

            for (int i = elevations.Count - 2; i >= 0; i--)
            {
                if (Mathf.Abs(elevations[i + 1] - elevations[i]) <= ElevationEpsilon)
                    elevations.RemoveAt(i + 1);
            }

            return elevations;
        }

        private static void GetVerticalBounds(SpatialVolumeSet volumeSet, out float bottom, out float top)
        {
            bottom = float.PositiveInfinity;
            top = float.NegativeInfinity;

            for (int i = 0; i < volumeSet.parts.Length; i++)
            {
                SpatialVolume part = volumeSet.parts[i];
                if (!IsValidVolume(part))
                    continue;

                bottom = Mathf.Min(bottom, part.bottom);
                top = Mathf.Max(top, part.top);
            }
        }

        private static SpatialVolume[] ClipVolumeSet(
            SpatialVolumeSet parentVolumeSet,
            SpatialBoundary[] boundaries,
            Vector2 insertPlanPoint,
            float bottom,
            float top)
        {
            var clippedParts = new List<SpatialVolume>();

            for (int i = 0; i < parentVolumeSet.parts.Length; i++)
            {
                SpatialVolume part = parentVolumeSet.parts[i];
                if (!IsValidVolume(part))
                    continue;

                float clippedBottom = Mathf.Max(part.bottom, bottom);
                float clippedTop = Mathf.Min(part.top, top);
                if (clippedTop - clippedBottom <= ElevationEpsilon)
                    continue;

                SpatialPlanRegion[] selectedRegions = SelectPlanRegions(part, boundaries, insertPlanPoint);
                for (int j = 0; j < selectedRegions.Length; j++)
                {
                    clippedParts.Add(new SpatialVolume
                    {
                        bottom = clippedBottom,
                        top = clippedTop,
                        points = CopyPoints(selectedRegions[j].points)
                    });
                }
            }

            return clippedParts.ToArray();
        }

        private static SpatialPlanRegion[] SelectPlanRegions(
            SpatialVolume parentPart,
            SpatialBoundary[] boundaries,
            Vector2 insertPlanPoint)
        {
            var initialRegion = new SpatialPlanRegion
            {
                points = parentPart.points
            };

            if (boundaries == null || boundaries.Length == 0)
                return new[] { initialRegion };

            if (!SpatialRegionSplitter.ContainsPoint(parentPart.points, insertPlanPoint))
                return System.Array.Empty<SpatialPlanRegion>();

            return SpatialRegionSplitter.SelectRegionsContainingPoint(initialRegion, boundaries, insertPlanPoint);
        }

        private static ref SpatialVolume AddOrGetVolume(int entity, EcsPool<SpatialVolume> volumePool)
        {
            if (volumePool.Has(entity))
                return ref volumePool.Get(entity);

            return ref volumePool.Add(entity);
        }

        private static ref SpatialVolumeSet AddOrGetVolumeSet(int entity, EcsPool<SpatialVolumeSet> volumeSetPool)
        {
            if (volumeSetPool.Has(entity))
                return ref volumeSetPool.Get(entity);

            return ref volumeSetPool.Add(entity);
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
