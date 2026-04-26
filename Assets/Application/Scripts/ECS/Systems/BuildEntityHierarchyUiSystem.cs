using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ECS
{
    public sealed class BuildEntityHierarchyUiSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var context = systems.GetShared<EcsAppContext>();

            if (context.EntityHierarchyPanel == null)
                return;

            var elementPool = world.GetPool<Element>();
            var hierarchyPool = world.GetPool<HierarchyNode>();

            var filter = world
                .Filter<Element>()
                .Inc<HierarchyNode>()
                .End();

            var childrenByParent = new Dictionary<int, List<int>>();
            var roots = new List<int>();

            foreach (int entity in filter)
            {
                ref var hierarchy = ref hierarchyPool.Get(entity);

                if (hierarchy.ParentEntity < 0)
                {
                    roots.Add(entity);
                    continue;
                }

                if (!childrenByParent.TryGetValue(hierarchy.ParentEntity, out var children))
                {
                    children = new List<int>();
                    childrenByParent.Add(hierarchy.ParentEntity, children);
                }

                children.Add(entity);
            }

            var rows = new List<EntityHierarchyRow>();

            foreach (int root in roots)
            {
                AddRowsRecursive(
                    root,
                    depth: 0,
                    rows,
                    childrenByParent,
                    elementPool
                );
            }

            context.EntityHierarchyPanel.Rebuild(rows);
        }

        private static void AddRowsRecursive(
            int entity,
            int depth,
            List<EntityHierarchyRow> rows,
            Dictionary<int, List<int>> childrenByParent,
            EcsPool<Element> elementPool)
        {
            ref var element = ref elementPool.Get(entity);

            rows.Add(new EntityHierarchyRow(
                entity,
                element.name,
                depth
            ));

            if (!childrenByParent.TryGetValue(entity, out var children))
                return;

            foreach (int child in children)
            {
                AddRowsRecursive(
                    child,
                    depth + 1,
                    rows,
                    childrenByParent,
                    elementPool
                );
            }
        }
    }
}