
namespace ECS
{
    public sealed class EcsAppContext
    {
        public Project Project { get; }
        public EcsEntityHierarchyPanel EntityHierarchyPanel { get; }

        public EcsAppContext(Project project, EcsEntityHierarchyPanel entityHierarchyPanel)
        {
            Project = project;
            EntityHierarchyPanel = entityHierarchyPanel;
        }
    }
}