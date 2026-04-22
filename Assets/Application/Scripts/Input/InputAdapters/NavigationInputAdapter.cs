using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationInputAdapter : MonoBehaviour
{
    [SerializeField] private AppStateController _appStateController;
    [Header("Editor actions")]
    [SerializeField] private InputActionReference _clickSelect;
    [SerializeField] private InputActionReference _multiSelectModifier;
    [SerializeField] private InputActionReference _cancelSelection;
    [Header("Shared pointer")]
    [SerializeField] private InputActionReference _pointerPosition;
    public bool IsActive =>
        _appStateController != null &&
        _appStateController.State.Value == AppState.Navigation;
    private void OnEnable()
    {
        _clickSelect?.action.Enable();
        _multiSelectModifier?.action.Enable();
        _cancelSelection?.action.Enable();
        _pointerPosition?.action.Enable();
    }
    private void OnDisable()
    {
        _clickSelect?.action.Disable();
        _multiSelectModifier?.action.Disable();
        _cancelSelection?.action.Disable();
        _pointerPosition?.action.Disable();
    }
    public NavigationInputFrame ReadFrame()
    {
        if (!IsActive)
            return default;
        return new NavigationInputFrame
        {
            PointerPosition = _pointerPosition != null
                ? _pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero,
            ClickSelectPressed = _clickSelect != null &&
                                 _clickSelect.action.WasPressedThisFrame(),
            MultiSelectHeld = _multiSelectModifier != null &&
                              _multiSelectModifier.action.IsPressed(),
            CancelSelectionPressed = _cancelSelection != null &&
                                     _cancelSelection.action.WasPressedThisFrame()
        };
    }
}