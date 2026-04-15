using NUnit.Framework.Constraints;
using UnityEngine;

public static class LineMeshBuilder
{
    public static Mesh BuildMesh(LineGeometry geometry)
    {
        Vector3 a = new Vector3()
        {
            x = geometry.Start.X,
            y = geometry.Start.Y,
            z = geometry.Start.Z
        };

        Vector3 b = new Vector3()
        {
            x = geometry.End.X,
            y = geometry.End.Y,
            z = geometry.End.Z
        };

        Vector3 dir = (b - a).normalized;
        Vector3 side = Vector3.Cross(dir, Vector3.up).normalized * 0.05f;

        if (side.sqrMagnitude < 0.0001f)
        {
            side = Vector3.Cross(dir, Vector3.right).normalized * 0.05f;
        }

        Vector3[] vertices = new Vector3[]
        {
            a - side,
            a + side,
            b - side,
            b + side
        };

        // uv.x = 0 или 1 (начало/конец линии)
        // uv.y = -1 или 1 (сторона линии)

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, -1),
            new Vector2(0, 1),
            new Vector2(1, -1),
            new Vector2(1, 1),
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3
        };

        Mesh mesh = new();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
        return mesh;
    }

}
