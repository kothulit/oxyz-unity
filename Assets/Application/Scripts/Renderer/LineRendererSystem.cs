using System.Collections.Generic;
using UnityEngine;

public class LineRendererSystem : MonoBehaviour
{
    private List<LineData> _lines = new List<LineData>();

    private Mesh _mesh;
    private Material _material;

    public void AddLine(LineData line)
    {
        _lines.Add(line);
    }

    public void Clear()
    {
        _lines.Clear();
    }

    void LateUpdate()
    {
        //BuildMesh();
        //Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, 0);
    }

    void BuildMesh()
    {
        int count = _lines.Count;

        var vertices = new Vector3[count * 4];
        var uv0 = new Vector2[count * 4]; // side + t
        var uv1 = new Vector4[count * 4]; // A
        var uv2 = new Vector4[count * 4]; // B
        var colors = new Color[count * 4];
        var uv3 = new Vector4[count * 4]; // thickness + dash

        var triangles = new int[count * 6];

        for (int i = 0; i < count; i++)
        {
            var line = _lines[i];
            int vi = i * 4;
            int ti = i * 6;

            vertices[vi + 0] = line.A;
            vertices[vi + 1] = line.A;
            vertices[vi + 2] = line.B;
            vertices[vi + 3] = line.B;

            uv0[vi + 0] = new Vector2(0, -1);
            uv0[vi + 1] = new Vector2(0, 1);
            uv0[vi + 2] = new Vector2(1, -1);
            uv0[vi + 3] = new Vector2(1, 1);

            uv1[vi + 0] = line.A;
            uv1[vi + 1] = line.A;
            uv1[vi + 2] = line.A;
            uv1[vi + 3] = line.A;

            uv2[vi + 0] = line.B;
            uv2[vi + 1] = line.B;
            uv2[vi + 2] = line.B;
            uv2[vi + 3] = line.B;

            uv3[vi + 0] = new Vector4(line.Thickness, line.DashSize, line.GapSize, 0);
            uv3[vi + 1] = uv3[vi + 0];
            uv3[vi + 2] = uv3[vi + 0];
            uv3[vi + 3] = uv3[vi + 0];

            colors[vi + 0] = line.Color;
            colors[vi + 1] = line.Color;
            colors[vi + 2] = line.Color;
            colors[vi + 3] = line.Color;

            triangles[ti + 0] = vi + 0;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = vi + 2;

            triangles[ti + 3] = vi + 2;
            triangles[ti + 4] = vi + 1;
            triangles[ti + 5] = vi + 3;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.SetUVs(0, uv0);
        _mesh.SetUVs(1, uv1);
        _mesh.SetUVs(2, uv2);
        _mesh.SetUVs(3, uv3);
        _mesh.colors = colors;
        _mesh.triangles = triangles;
    }
}