
namespace ECS
{
    public sealed class EcsAppContext
    {
        public Project Project { get; }

        public EcsAppContext(Project project)
        {
            Project = project;
        }
    }
}