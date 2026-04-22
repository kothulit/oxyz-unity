using UnityEngine;
using UnityEngine.InputSystem;

public class CreationInputAdapter : MonoBehaviour
{
    [SerializeField] private AppStateController _appStateController;
    [Header("Editor actions")]
    [SerializeField] private InputActionReference _place;
    [SerializeField] private InputActionReference _confirm;
    [SerializeField] private InputActionReference _cancel;
    [Header("Shared pointer")]
    [SerializeField] private InputActionReference _pointerPosition;
    public bool IsActive =>
        _appStateController != null &&
        _appStateController.State.Value == AppState.Creating;
    private void OnEnable()
    {
        _place?.action.Enable();
        _confirm?.action.Enable();
        _cancel?.action.Enable();
        _pointerPosition?.action.Enable();
    }
    private void OnDisable()
    {
        _place?.action.Disable();
        _confirm?.action.Disable();
        _cancel?.action.Disable();
        _pointerPosition?.action.Disable();
    }
    public CreationInputFrame ReadFrame()
    {
        if (!IsActive)
            return default;
        return new CreationInputFrame
        {
            PointerPosition = _pointerPosition != null
                ? _pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero,
            PlacePressed = _place != null &&
                           _place.action.WasPressedThisFrame(),
            ConfirmPressed = _confirm != null &&
                             _confirm.action.WasPressedThisFrame(),
            CancelPressed = _cancel != null &&
                            _cancel.action.WasPressedThisFrame()
        };
    }
}