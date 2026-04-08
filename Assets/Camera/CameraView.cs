using UnityEngine;

public sealed class CameraView : MonoBehaviour
{
    public void Apply(CameraPose pose)
    {
        transform.SetPositionAndRotation(pose.Position, pose.Rotation);
    }
}