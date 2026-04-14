using R3;
using UnityEngine;

public class AppStateController : MonoBehaviour
{
    public ReactiveProperty<AppState> State = new(AppState.Navigation);
}