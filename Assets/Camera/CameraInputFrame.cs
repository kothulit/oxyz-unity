using UnityEngine;

public struct CameraInputFrame
{
    public Vector2 PointerDelta;
    public float ZoomDelta;
    public bool IsPanning;
    public bool IsOrbiting;
    public bool FocusPressed;
}