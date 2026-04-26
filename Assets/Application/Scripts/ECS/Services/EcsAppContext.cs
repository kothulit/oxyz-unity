
using UnityEngine;

namespace ECS
{
    public sealed class EcsAppContext
    {
        public Transform ViewRoot { get; }
        public Material DefaultMaterial { get; }
        public Project Project { get; }
        public EcsEntityHierarchyPanel EntityHierarchyPanel { get; }

        public EcsAppContext(
            Project project,
            EcsEntityHierarchyPanel entityHierarchyPanel,
            Transform viewRoot,
            Material defaultMaterial)
        {
            Project = project;
            EntityHierarchyPanel = entityHierarchyPanel;
            ViewRoot = viewRoot;
            DefaultMaterial = defaultMaterial; 
        }
    }
}