using System.Collections.Generic;
using UnityEngine;
using Oxyz.Xml.Serializable;

public static class ExtrusionMeshBuilder
{
    public static Mesh BuildMesh(ExtrusionGeometry geometry)
    {
        //var contour = NormalizeContourWinding(ExtractOrderedContourPoints(geometry));
        //if (contour.Count < 3)
        //    return null;
        //float yBottom = geometry.InsertPoint.Y + geometry.BottomOffset;
        //float yTop = geometry.InsertPoint.Y + geometry.TopOffset;
        //var vertices = new List<Vector3>();
        //var triangles = new List<int>();
        //AddBottomFace(vertices, triangles, contour, geometry.InsertPoint, yBottom);
        //AddTopFace(vertices, triangles, contour, geometry.InsertPoint, yTop);
        //AddSideFaces(vertices, triangles, contour, geometry.InsertPoint, yBottom, yTop);
        //var mesh = new Mesh();
        //mesh.name = geometry.Id.ToString() ?? "ExtrusionMesh";
        //mesh.SetVertices(vertices);
        //mesh.SetTriangles(triangles, 0);
        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        return new Mesh(); //mesh;
    }

    private static void AddBottomFace(
        List<Vector3> vertices,
        List<int> triangles,
        List<Point2D> contour,
        Point3D insertPoint,
        float yBottom)
    {
        int startIndex = vertices.Count;

        for (int i = 0; i < contour.Count; i++)
        {
            var p = contour[i];
            vertices.Add(new Vector3(
                insertPoint.X + p.X,
                yBottom,
                insertPoint.Z + p.Y));
        }

        for (int i = 1; i < contour.Count - 1; i++)
        {
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + i);
            triangles.Add(startIndex + i + 1);
        }
    }

    private static void AddTopFace(
        List<Vector3> vertices,
        List<int> triangles,
        List<Point2D> contour,
        Point3D insertPoint,
        float yTop)
    {
        int startIndex = vertices.Count;

        for (int i = 0; i < contour.Count; i++)
        {
            var p = contour[i];
            vertices.Add(new Vector3(
                insertPoint.X + p.X,
                yTop,
                insertPoint.Z + p.Y));
        }

        for (int i = 1; i < contour.Count - 1; i++)
        {
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + i + 1);
            triangles.Add(startIndex + i);
        }
    }

    private static void AddSideFaces(
        List<Vector3> vertices,
        List<int> triangles,
        List<Point2D> contour,
        Point3D insertPoint,
        float yBottom,
        float yTop)
    {
        int count = contour.Count;

        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            var p0 = contour[i];
            var p1 = contour[next];

            Vector3 b0 = new Vector3(insertPoint.X + p0.X, yBottom, insertPoint.Z + p0.Y);
            Vector3 b1 = new Vector3(insertPoint.X + p1.X, yBottom, insertPoint.Z + p1.Y);
            Vector3 t0 = new Vector3(insertPoint.X + p0.X, yTop, insertPoint.Z + p0.Y);
            Vector3 t1 = new Vector3(insertPoint.X + p1.X, yTop, insertPoint.Z + p1.Y);

            int startIndex = vertices.Count;

            vertices.Add(b0);
            vertices.Add(b1);
            vertices.Add(t1);
            vertices.Add(t0);

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 3);
            triangles.Add(startIndex + 2);
        }
    }

    private static List<Point2D> ExtractOrderedContourPoints(ExtrusionGeometry geometry)
    {
        //var result = new List<Point2D>();

        //if (geometry?.Contour == null || geometry.Contour.Count == 0)
        //    return result;

        //for (int i = 0; i < geometry.Contour.Count; i++)
        //{
        //    result.Add(geometry.Contour[i].Start);
        //}

        return new List<Point2D>();  //result;
    }

    private static float GetSignedArea(List<Point2D> contour)
    {
        float area = 0f;
        for (int i = 0; i < contour.Count; i++)
        {
            Point2D current = contour[i];
            Point2D next = contour[(i + 1) % contour.Count];
            area += current.X * next.Y - next.X * current.Y;
        }
        return area * 0.5f;
    }

    private static List<Point2D> NormalizeContourWinding(List<Point2D> contour)
    {
        var normalized = new List<Point2D>(contour);
        float signedArea = GetSignedArea(normalized);
        if (signedArea < 0f)
        {
            normalized.Reverse();
        }
        return normalized;
    }
}