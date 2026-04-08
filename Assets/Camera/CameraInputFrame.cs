using UnityEngine;

public struct CameraInputFrame
{
    public Vector2 PointerDelta;
    public Vector2 PointerPosition;
    public float ZoomDelta;
    public bool IsPanning;
    public bool IsOrbiting;
    public bool FocusPressed;
}