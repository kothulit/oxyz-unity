using ECS.Tag;
using Leopotam.EcsLite;
using Oxyz.Xml.Serializable;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance.AvailableData;

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
            EcsPool<Element> elementPool = world.GetPool<Element>();
            EcsPool<ExtrusionGeometry> extrusionGeometryPool = world.GetPool<ExtrusionGeometry>();
            EcsPool<Geometry> geometryPool = world.GetPool<Geometry>();
            EcsPool<HierarchyNode> hierarchyPool = world.GetPool<HierarchyNode>();

            SiteElement host = project.Document.Site;

            for (int i = 0; i < project.Document.Site.Buildings.Count; i++)
            {
                BuildingElement building = project.Document.Site.Buildings[i];
                Oxyz.Xml.Serializable.ExtrusionGeometry extrusion = building.Extrusion;

                int entity = world.NewEntity();

                buildingTagPool.Add(entity);

                ref Element element = ref elementPool.Add(entity);
                element.name = building.Name;
                element.sourceIndex = i;
                element.guid = building.GUID;
                element.hostName = host.Name;

                ref ExtrusionGeometry extrusionGeometry = ref extrusionGeometryPool.Add(entity);
                extrusionGeometry.bottom = extrusion.Bottom;
                extrusionGeometry.top = extrusion.Top;
                extrusionGeometry.points = new Vector2[extrusion.Loop.Length];
                for (int j = 0; j < extrusion.Loop.Length; j++)
                {
                    extrusionGeometry.points[j] = new Vector2(extrusion.Loop[j].X, extrusion.Loop[j].Y);
                }

                ref Geometry geometry = ref geometryPool.Add(entity);

                ref HierarchyNode hierarchy = ref hierarchyPool.Add(entity);
                hierarchy.ParentEntity = siteEntity;

                Debug.Log($"[ECS] Building entity created: {element.name}");
            }
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
