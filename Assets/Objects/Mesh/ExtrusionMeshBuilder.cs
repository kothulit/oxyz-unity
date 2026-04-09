using System.Collections.Generic;
using UnityEngine;

public static class ExtrusionMeshBuilder
{
    public static Mesh BuildMesh(ExtrusionGeometry geometry)
    {
        var contour = ExtractOrderedContourPoints(geometry);
        if (contour.Count < 3)
            return null;
        float yBottom = geometry.InsertPoint.Y + geometry.BottomOffset;
        float yTop = geometry.InsertPoint.Y + geometry.TopOffset;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        int n = contour.Count;
        // Bottom vertices
        for (int i = 0; i < n; i++)
        {
            var p = contour[i];
            vertices.Add(new Vector3(
                geometry.InsertPoint.X + p.X,
                yBottom,
                geometry.InsertPoint.Z + p.Y));
        }
        // Top vertices
        for (int i = 0; i < n; i++)
        {
            var p = contour[i];
            vertices.Add(new Vector3(
                geometry.InsertPoint.X + p.X,
                yTop,
                geometry.InsertPoint.Z + p.Y));
        }
        // Bottom face
        for (int i = 1; i < n - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        // Top face
        int topOffset = n;
        for (int i = 1; i < n - 1; i++)
        {
            triangles.Add(topOffset + 0);
            triangles.Add(topOffset + i + 1);
            triangles.Add(topOffset + i);

        }
        // Side faces
        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;
            int b0 = i;
            int b1 = next;
            int t0 = topOffset + i;
            int t1 = topOffset + next;
            triangles.Add(b0);
            triangles.Add(t0);
            triangles.Add(t1);
            triangles.Add(b0);
            triangles.Add(t1);
            triangles.Add(b1);
        }
        var mesh = new Mesh();
        mesh.name = geometry.Id ?? "ExtrusionMesh";
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
    private static List<Point2D> ExtractOrderedContourPoints(ExtrusionGeometry geometry)
    {
        var result = new List<Point2D>();
        if (geometry?.Contour == null || geometry.Contour.Count == 0)
            return result;
        for (int i = 0; i < geometry.Contour.Count; i++)
        {
            result.Add(geometry.Contour[i].Start);
        }
        return result;
    }
}