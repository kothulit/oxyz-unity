using System;

public sealed class ProjectSession
{
    public Project CurrentProject { get; private set; }

    public bool HasProject => CurrentProject != null;

    public event Action<Project> ProjectChanged;

    public void SetCurrentProject(Project project)
    {
        CurrentProject = project ?? throw new ArgumentNullException(nameof(project));
        ProjectChanged?.Invoke(CurrentProject);
    }

    public void Clear()
    {
        CurrentProject = null;
        ProjectChanged?.Invoke(null);
    }
}