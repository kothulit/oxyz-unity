using R3;
using System;

public sealed class ProjectSession : IDisposable
{
    public ReactiveProperty<Project> CurrentProject { get; private set; } = new(null);

    public bool HasProject => CurrentProject.Value != null;

    public void SetCurrentProject(Project project)
    {
        CurrentProject.OnNext(project ?? throw new ArgumentNullException(nameof(project)));
    }

    public void Clear()
    {
        CurrentProject.OnNext(null);

    }

    public void Dispose()
    {
        CurrentProject.Dispose();
    }
}