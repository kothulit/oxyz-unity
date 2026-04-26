using Client;
using UnityEngine;
using VContainer.Unity;

public class AppStartup : IStartable
{
    private AppStateController _appStateController;
    private readonly IProjectLoader _projectLoader;
    private readonly ProjectSession _projectSession;
    private readonly EcsRuntime _ecsStartup;

    public AppStartup(
        AppStateController appStateController,
        IProjectLoader projectLoader,
        ProjectSession projectSession,
        EcsRuntime ecsStartup)
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
    }
}