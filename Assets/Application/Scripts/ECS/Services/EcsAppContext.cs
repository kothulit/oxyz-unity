
namespace ECS
{
    public sealed class EcsAppContext
    {
        public ProjectSession ProjectSession { get; }

        public EcsAppContext(ProjectSession projectSession)
        {
            ProjectSession = projectSession;
        }
    }
}