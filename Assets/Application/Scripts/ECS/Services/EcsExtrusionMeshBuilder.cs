using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public static class EcsExtrusionMeshBuilder
    {
        public static Mesh Build(ExtrusionGeometry geometry)
        {
            if (geometry.points == null || geometry.points.Length < 3)
                return null;

            List<Vector2> points = NormalizeWinding(new List<Vector2>(geometry.points));

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            AddBottomFace(vertices, triangles, points, geometry.bottom);
            AddTopFace(vertices, triangles, points, geometry.top);
            AddSideFaces(vertices, triangles, points, geometry.bottom, geometry.top);

            var mesh = new Mesh();
            mesh.name = "BuildingExtrusionMesh";
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void AddBottomFace(List<Vector3> vertices, List<int> triangles, List<Vector2> points, float y)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(new Vector3(p.x, y, p.y));

            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(start + 0);
                triangles.Add(start + i);
                triangles.Add(start + i + 1);
            }
        }

        private static void AddTopFace(List<Vector3> vertices, List<int> triangles, List<Vector2> points, float y)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(new Vector3(p.x, y, p.y));

            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(start + 0);
                triangles.Add(start + i + 1);
                triangles.Add(start + i);
            }
        }

        private static void AddSideFaces(List<Vector3> vertices, List<int> triangles, List<Vector2> points, float bottom, float top)
        {
            for (int i = 0; i < points.Count; i++)
            {
                int next = (i + 1) % points.Count;

                Vector2 p0 = points[i];
                Vector2 p1 = points[next];

                int start = vertices.Count;

                vertices.Add(new Vector3(p0.x, bottom, p0.y));
                vertices.Add(new Vector3(p1.x, bottom, p1.y));
                vertices.Add(new Vector3(p1.x, top, p1.y));
                vertices.Add(new Vector3(p0.x, top, p0.y));

                triangles.Add(start + 0);
                triangles.Add(start + 2);
                triangles.Add(start + 1);

                triangles.Add(start + 0);
                triangles.Add(start + 3);
                triangles.Add(start + 2);
            }
        }

        private static List<Vector2> NormalizeWinding(List<Vector2> points)
        {
            if (GetSignedArea(points) < 0f)
                points.Reverse();

            return points;
        }

        private static float GetSignedArea(List<Vector2> points)
        {
            float area = 0f;

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 current = points[i];
                Vector2 next = points[(i + 1) % points.Count];

                area += current.x * next.y - next.x * current.y;
            }

            return area * 0.5f;
        }
    }
}