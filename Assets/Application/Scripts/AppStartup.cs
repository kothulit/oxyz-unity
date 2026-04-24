using UnityEngine;
using VContainer.Unity;
public class AppStartup : IStartable
{
    private AppStateController _appStateController;
    private readonly IProjectLoader _projectLoader;

    public AppStartup(AppStateController appStateController, IProjectLoader projectLoader)
    {
        _appStateController = appStateController;
        _appStateController.State.OnNext(AppState.Navigation);
        _projectLoader = projectLoader;
    }

    public void Start()
    {
        Debug.Log("[AppStartup] VContainer start");

        var project = _projectLoader.Load(@"C:\GitSourceTree\oxyz-unity\Assets\Examples\XMLTestProject");

        Debug.Log($"Project loaded: {project.Document.Name}");
        Debug.Log($"Styles: {project.StylesByName.Count}");
        Debug.Log($"Materials: {project.MaterialsByName.Count}");
    }
}