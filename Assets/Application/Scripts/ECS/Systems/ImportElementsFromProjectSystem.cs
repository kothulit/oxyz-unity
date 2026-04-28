using System.Collections.Generic;
using ECS.Tag;
using Leopotam.EcsLite;
using Oxyz.Xml.Serializable;
using UnityEngine;

namespace ECS
{
    public class ImportElementsFromProjectSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            EcsAppContext context = systems.GetShared<EcsAppContext>();
            int siteEntity = -1;

            Project project = context.Project;

            if (project == null)
            {
                Debug.LogWarning("[ECS] No project.");
                return;
            }

            Debug.Log($"[ECS] Import project: {project.Document.Name}");

            if (project.Document?.Site?.Buildings == null)
            {
                Debug.LogWarning("[ECS] Project has no buildings.");
                return;
            }

            siteEntity = ImportSite(world, project);

            ImportBuilding(world, project, siteEntity);
        }

        private void ImportBuilding(EcsWorld world, Project project, int siteEntity)
        {
            if (project.Document?.Site?.Buildings == null)
            {
                Debug.LogWarning("[ECS] Project has no buildings.");
                return;
            }

            EcsPool<BuildingTag> buildingTagPool = world.GetPool<BuildingTag>();
            EcsPool<SpaceTag> spaceTagPool = world.GetPool<SpaceTag>();
            EcsPool<Element> elementPool = world.GetPool<Element>();
            EcsPool<SpatialVolume> volumePool = world.GetPool<SpatialVolume>();
            EcsPool<SpatialVolumeSet> volumeSetPool = world.GetPool<SpatialVolumeSet>();
            EcsPool<SpatialContainer> containerPool = world.GetPool<SpatialContainer>();
            EcsPool<SpatialDivision> divisionPool = world.GetPool<SpatialDivision>();
            EcsPool<SpacePlacement> placementPool = world.GetPool<SpacePlacement>();
            EcsPool<SpatialDefinitionSource> sourcePool = world.GetPool<SpatialDefinitionSource>();
            EcsPool<HierarchyNode> hierarchyPool = world.GetPool<HierarchyNode>();

            SiteElement host = project.Document.Site;

            for (int i = 0; i < project.Document.Site.Buildings.Count; i++)
            {
                BuildingElement building = project.Document.Site.Buildings[i];
                SpatialVolume[] volumes = GetValidVolumes(building);

                if (volumes.Length == 0)
                {
                    Debug.LogWarning($"[ECS] Building skipped. Invalid spatial definition: {building.Name}");
                    continue;
                }

                int entity = world.NewEntity();

                buildingTagPool.Add(entity);
                containerPool.Add(entity);

                ref Element element = ref elementPool.Add(entity);
                element.name = building.Name;
                element.sourceIndex = i;
                element.guid = building.GUID;
                element.hostName = host.Name;

                ref SpatialVolumeSet volumeSet = ref volumeSetPool.Add(entity);
                volumeSet.parts = volumes;

                ref SpatialVolume volume = ref volumePool.Add(entity);
                volume = volumes[0];

                ref HierarchyNode hierarchy = ref hierarchyPool.Add(entity);
                hierarchy.ParentEntity = siteEntity;

                ref SpatialDivision division = ref divisionPool.Add(entity);
                division.dividingPlaneElevations = GetDividingPlaneElevations(building.DividingPlanes);

                ref SpatialDefinitionSource source = ref sourcePool.Add(entity);
                source.sourceIndex = i;
                source.sourceType = "Building";

                Debug.Log($"[ECS] Building entity created: {element.name}");

                ImportSpaces(
                    world,
                    building.Spaces,
                    entity,
                    element.name,
                    spaceTagPool,
                    elementPool,
                    containerPool,
                    divisionPool,
                    placementPool,
                    sourcePool,
                    hierarchyPool);
            }
        }

        private static void ImportSpaces(
            EcsWorld world,
            List<SpaceElement> spaces,
            int parentEntity,
            string hostName,
            EcsPool<SpaceTag> spaceTagPool,
            EcsPool<Element> elementPool,
            EcsPool<SpatialContainer> containerPool,
            EcsPool<SpatialDivision> divisionPool,
            EcsPool<SpacePlacement> placementPool,
            EcsPool<SpatialDefinitionSource> sourcePool,
            EcsPool<HierarchyNode> hierarchyPool)
        {
            if (spaces == null)
                return;

            for (int i = 0; i < spaces.Count; i++)
            {
                SpaceElement space = spaces[i];
                int entity = world.NewEntity();

                spaceTagPool.Add(entity);
                containerPool.Add(entity);

                ref Element element = ref elementPool.Add(entity);
                element.name = space.Name;
                element.sourceIndex = i;
                element.guid = space.GUID;
                element.hostName = hostName;

                ref SpacePlacement placement = ref placementPool.Add(entity);
                placement.insertPoint = ToVector3(space.InsertPoint);

                ref SpatialDivision division = ref divisionPool.Add(entity);
                division.dividingPlaneElevations = GetDividingPlaneElevations(space.DividingPlanes);

                ref SpatialDefinitionSource source = ref sourcePool.Add(entity);
                source.sourceIndex = i;
                source.sourceType = "Space";

                ref HierarchyNode hierarchy = ref hierarchyPool.Add(entity);
                hierarchy.ParentEntity = parentEntity;

                Debug.Log($"[ECS] Space entity created: {element.name}");

                ImportSpaces(
                    world,
                    space.Spaces,
                    entity,
                    element.name,
                    spaceTagPool,
                    elementPool,
                    containerPool,
                    divisionPool,
                    placementPool,
                    sourcePool,
                    hierarchyPool);
            }
        }

        private static SpatialVolume[] GetValidVolumes(BuildingElement building)
        {
            if (building.Geometry?.Extrusions == null || building.Geometry.Extrusions.Count == 0)
                return System.Array.Empty<SpatialVolume>();

            var volumes = new List<SpatialVolume>();

            for (int i = 0; i < building.Geometry.Extrusions.Count; i++)
            {
                Oxyz.Xml.Serializable.ExtrusionGeometry extrusion = building.Geometry.Extrusions[i];
                if (extrusion?.Loop == null || extrusion.Loop.Length < 3 || extrusion.Top <= extrusion.Bottom)
                {
                    Debug.LogWarning($"[ECS] Building extrusion skipped. Invalid extrusion #{i + 1}: {building.Name}");
                    continue;
                }

                volumes.Add(new SpatialVolume
                {
                    bottom = extrusion.Bottom,
                    top = extrusion.Top,
                    points = ToVector2Array(extrusion.Loop)
                });
            }

            return volumes.ToArray();
        }

        private static Vector2[] ToVector2Array(CheckPoint[] points)
        {
            Vector2[] result = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = new Vector2(points[i].X, points[i].Y);
            }

            return result;
        }

        private static Vector3 ToVector3(Point3D point)
        {
            if (point == null)
                return Vector3.zero;

            return new Vector3(point.X, point.Y, point.Z);
        }

        private static float[] GetDividingPlaneElevations(List<DividingPlane> dividingPlanes)
        {
            if (dividingPlanes == null || dividingPlanes.Count == 0)
                return System.Array.Empty<float>();

            float[] elevations = new float[dividingPlanes.Count];
            for (int i = 0; i < dividingPlanes.Count; i++)
            {
                elevations[i] = dividingPlanes[i].InsertPoint.Z;
            }

            return elevations;
        }

        private int ImportSite(EcsWorld world, Project project)
        {
            if (project.Document?.Site == null)
            {
                Debug.LogWarning("[ECS] Project has no site.");
                return -1;
            }

            EcsPool<SiteTag> siteTagPool = world.GetPool<SiteTag>();
            EcsPool<Element> elementPool = world.GetPool<Element>();
            EcsPool<HierarchyNode> hierarchyPool = world.GetPool<HierarchyNode>();

            int siteEntity = world.NewEntity();

            siteTagPool.Add(siteEntity);

            ref Element siteElement = ref elementPool.Add(siteEntity);
            siteElement.name = project.Document.Site.Name;
            siteElement.guid = project.Document.Site.GUID;
            siteElement.sourceIndex = 0;

            ref HierarchyNode siteHierarchy = ref hierarchyPool.Add(siteEntity);
            siteHierarchy.ParentEntity = -1;

            Debug.Log($"[ECS] Site entity created: {siteElement.name}");

            return siteEntity;
        }
    }
}
