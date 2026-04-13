using System;
using UnityEngine;

public class AppStateController : MonoBehaviour
{
    public AppState CurrentState { get; private set; } = AppState.Navigation;
    public event Action<AppState> StateChanged;
    public void SetState(AppState newState)
    {
        if (CurrentState == newState)
            return;
        CurrentState = newState;
        StateChanged?.Invoke(CurrentState);
    }
}