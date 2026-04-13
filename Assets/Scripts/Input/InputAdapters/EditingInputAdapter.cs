using UnityEngine;
using UnityEngine.InputSystem;

public class EditingInputAdapter : MonoBehaviour
{
    [SerializeField] private AppStateController _appStateController;
    [Header("Editor actions")]
    [SerializeField] private InputActionReference _clickSelect;
    [SerializeField] private InputActionReference _beginDrag;
    [SerializeField] private InputActionReference _confirm;
    [SerializeField] private InputActionReference _cancel;
    [SerializeField] private InputActionReference _delete;
    [Header("Shared pointer")]
    [SerializeField] private InputActionReference _pointerPosition;
    public bool IsActive =>
        _appStateController != null &&
        _appStateController.CurrentState == AppState.Editing;
    private void OnEnable()
    {
        _clickSelect?.action.Enable();
        _beginDrag?.action.Enable();
        _confirm?.action.Enable();
        _cancel?.action.Enable();
        _delete?.action.Enable();
        _pointerPosition?.action.Enable();
    }
    private void OnDisable()
    {
        _clickSelect?.action.Disable();
        _beginDrag?.action.Disable();
        _confirm?.action.Disable();
        _cancel?.action.Disable();
        _delete?.action.Disable();
        _pointerPosition?.action.Disable();
    }
    public EditingInputFrame ReadFrame()
    {
        if (!IsActive)
            return default;
        return new EditingInputFrame
        {
            PointerPosition = _pointerPosition != null
                ? _pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero,
            ClickSelectPressed = _clickSelect != null &&
                                 _clickSelect.action.WasPressedThisFrame(),
            BeginDragPressed = _beginDrag != null &&
                               _beginDrag.action.WasPressedThisFrame(),
            ConfirmPressed = _confirm != null &&
                             _confirm.action.WasPressedThisFrame(),
            CancelPressed = _cancel != null &&
                            _cancel.action.WasPressedThisFrame(),
            DeletePressed = _delete != null &&
                            _delete.action.WasPressedThisFrame()
        };
    }
}