using Unity.VisualScripting;
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
    [SerializeField] private float _panSpeed = 0.01f;
    [SerializeField] private float _zoomSpeed = 0.5f;

    [Header("Limits")]
    [SerializeField] private float _minPitch = -80f;
    [SerializeField] private float _maxPitch = 80f;
    [SerializeField] private float _minDistance = 0.5f;
    [SerializeField] private float _maxDistance = 200f;

    public void Tick(CameraInputFrame input)
    {
        if (input.IsOrbiting)
        {
            _yaw += input.PointerDelta.x * _orbitSpeed;
            _pitch -= input.PointerDelta.x * _orbitSpeed;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        if (input.IsPanning)
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;

            Vector3 worldDelta =
                (-right * input.PointerDelta.x + -up * input.PointerDelta.y) * _panSpeed * Mathf.Max(1f, _distance);

            _pivot += worldDelta;
        }

        if (Mathf.Abs(input.ZoomDelta) > Mathf.Epsilon)
        {
            _distance -= input.ZoomDelta * _zoomSpeed;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }
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

    public void SerPivot(Vector3 newPivot)
    {
        _pivot = newPivot;
    }
}
