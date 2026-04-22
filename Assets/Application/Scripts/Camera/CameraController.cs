using UnityEngine;
[System.Serializable]
public class CameraController
{
    [Header("State")]
    [SerializeField] private Vector3 _pivot = Vector3.zero;
    [SerializeField] private float _distance = 10f;
    [SerializeField] private float _yaw = 45f;
    [SerializeField] private float _pitch = 30f;
    [Header("Speeds")]
    [SerializeField] private float _orbitSpeed = 0.2f;
    [SerializeField] private float _panSpeed = 1f;
    [SerializeField] private float _zoomSpeed = 0.5f;
    [Header("Limits")]
    [SerializeField] private float _minPitch = -90f;
    [SerializeField] private float _maxPitch = 90f;
    [SerializeField] private float _minDistance = 0.5f;
    [SerializeField] private float _maxDistance = 200f;
    public void Tick(CameraInputFrame input, Camera sceneCamera)
    {
        if (input.IsOrbiting)
        {
            _yaw += input.PointerDelta.x * _orbitSpeed;
            _pitch -= input.PointerDelta.y * _orbitSpeed;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }
        if (input.IsPanning)
        {
            PanInViewPlane(input.PointerDelta, sceneCamera);
        }
        if (Mathf.Abs(input.ZoomDelta) > Mathf.Epsilon)
        {
            _distance -= input.ZoomDelta * _zoomSpeed;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }
    }
    private void PanInViewPlane(Vector2 pointerDelta, Camera sceneCamera)
    {
        if (sceneCamera == null)
            return;
        float distance = Mathf.Max(_distance, 0.001f);
        float frustumHeight = 2f * distance * Mathf.Tan(sceneCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * sceneCamera.aspect;
        float unitsPerPixelX = frustumWidth / sceneCamera.pixelWidth;
        float unitsPerPixelY = frustumHeight / sceneCamera.pixelHeight;
        Vector3 move =
            (-sceneCamera.transform.right * pointerDelta.x * unitsPerPixelX) +
            (-sceneCamera.transform.up * pointerDelta.y * unitsPerPixelY);
        _pivot += move * _panSpeed;
    }
    public CameraPose GetPose()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 position = _pivot - rotation * Vector3.forward * _distance;
        return new CameraPose(position, rotation);
    }
    public void Focus(Bounds bounds)
    {
        _pivot = bounds.center;
        float radius = bounds.extents.magnitude;
        _distance = Mathf.Clamp(radius * 2.5f, _minDistance, _maxDistance);
    }
    public void SetPivot(Vector3 newPivot)
    {
        _pivot = newPivot;
    }
}