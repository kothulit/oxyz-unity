using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputAdapter : MonoBehaviour
{
    [Header("Action References")]
    [SerializeField] private InputActionReference _panModifier;
    [SerializeField] private InputActionReference _orbitModifier;
    [SerializeField] private InputActionReference _pointerDelta;
    [SerializeField] private InputActionReference _zoom;
    [SerializeField] private InputActionReference _focusSelection;
    private void OnEnable()
    {
        _panModifier?.action.Enable();
        _orbitModifier?.action.Enable();
        _pointerDelta?.action.Enable();
        _zoom?.action.Enable();
        _focusSelection?.action.Enable();
    }
    private void OnDisable()
    {
        _panModifier?.action.Disable();
        _orbitModifier?.action.Disable();
        _pointerDelta?.action.Disable();
        _zoom?.action.Disable();
        _focusSelection?.action.Disable();
    }
    public CameraInputFrame ReadFrame()
    {
        return new CameraInputFrame
        {
            PointerDelta = _pointerDelta != null ? _pointerDelta.action.ReadValue<Vector2>() : Vector2.zero,
            ZoomDelta = _zoom != null ? _zoom.action.ReadValue<Vector2>().y : 0f,
            IsPanning = _panModifier != null && _panModifier.action.IsPressed(),
            IsOrbiting = _orbitModifier != null && _orbitModifier.action.IsPressed(),
            FocusPressed = _focusSelection != null && _focusSelection.action.WasPressedThisFrame()
        };
    }
}
