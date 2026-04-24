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
    }
}