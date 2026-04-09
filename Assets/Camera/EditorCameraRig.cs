using UnityEngine;

public sealed class EditorCameraRig : MonoBehaviour
{
    [SerializeField] private CameraInputAdapter _inputAdapter;
    [SerializeField] private CameraView _cameraView;
    [SerializeField] private Camera _camera;

    [Header("Optional focus target")]
    [SerializeField] private Renderer _focusRenderer;

    [SerializeField] private CameraController _controller = new CameraController();

    private void Reset()
    {
        _cameraView = GetComponentInChildren<CameraView>();
        _camera = GetComponentInChildren<Camera>();
        _inputAdapter = GetComponent<CameraInputAdapter>();
    }

    private void Update()
    {
        CameraInputFrame input = _inputAdapter.ReadFrame();
        if (input.FocusPressed && _focusRenderer != null)
        {
            _controller.Focus(_focusRenderer.bounds);
        }
        _controller.Tick(input, _camera);
        _cameraView.Apply(_controller.GetPose());
    }
}