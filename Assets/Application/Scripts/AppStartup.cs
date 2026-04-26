using Client;
using System.IO;
using UnityEngine;
using VContainer.Unity;
public class AppStartup : IStartable
{
    private AppStateController _appStateController;
    private readonly IProjectLoader _projectLoader;
    private readonly ProjectSession _projectSession;
    private readonly EcsStartup _ecsStartup;

    public AppStartup(
        AppStateController appStateController,
        IProjectLoader projectLoader,
        ProjectSession projectSession,
        EcsStartup ecsStartup)
    {
        _appStateController = appStateController;
        _projectLoader = projectLoader;
        _projectSession = projectSession;
        _ecsStartup = ecsStartup;
    }

    public void Start()
    {
        Debug.Log("[AppStartup] VContainer start");

        _appStateController.State.OnNext(AppState.Navigation);

        string projectPath = Path.Combine(
            Application.dataPath,
            "Examples",
            "XMLTestProject"
        );

        Project project = _projectLoader.Load(projectPath);
        _projectSession.SetCurrentProject(project);

        Debug.Log($"Project loaded: {project.Document.Name}");
        Debug.Log($"Styles: {project.StylesByName.Count}");
        Debug.Log($"Materials: {project.MaterialsByName.Count}");

        _ecsStartup.Initialize();
    }
}