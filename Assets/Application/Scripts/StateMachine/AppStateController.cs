using R3;

public class AppStateController
{
    public ReactiveProperty<AppState> State = new(AppState.Navigation);
}