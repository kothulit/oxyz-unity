using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class AppStateFrameController : MonoBehaviour
{
    private AppStateController _appStateController;

    [Header("Frame images")]
    [SerializeField] private Image _topBorder;
    [SerializeField] private Image _bottomBorder;
    [SerializeField] private Image _leftBorder;
    [SerializeField] private Image _rightBorder;

    [Header("Colors")]
    [SerializeField] private Color _navigationColor = Color.green;
    [SerializeField] private Color _editingColor = Color.red;
    [SerializeField] private Color _creatingColor = Color.blue;
    [SerializeField] private Color _fallbackColor = Color.gray;

    [Inject]
    private void Construct(AppStateController appStateController)
    {
        _appStateController = appStateController;
    }

    private void OnEnable()
    {
        if (_appStateController != null)
        {
            _appStateController.State.Subscribe(OnAppStateChanged);
        }
    }

    private void OnDisable()
    {
        _appStateController.State.Dispose();
    }

    private void OnAppStateChanged(AppState state)
    {
        ApplyStateColor(state);
    }

    private void ApplyStateColor(AppState state)
    {
        Color color = state switch
        {
            AppState.Navigation => _navigationColor,
            AppState.Creating => _creatingColor,
            AppState.Editing => _editingColor,
            _ => _fallbackColor
        };
        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        if (_topBorder != null)
            _topBorder.color = color;
        if (_bottomBorder != null)
            _bottomBorder.color = color;
        if (_leftBorder != null)
            _leftBorder.color = color;
        if (_rightBorder != null)
            _rightBorder.color = color;
    }
}
