using UnityEngine;
using VContainer.Unity;
public class AppStartup : IStartable
{
    private AppStateController _appStateController;

    public AppStartup(AppStateController appStateController)
    {
        _appStateController = appStateController;
        _appStateController.State.OnNext(AppState.Navigation);
    }
    public void Start()
    {
        Debug.Log("[AppStartup] VContainer start");
    }
}