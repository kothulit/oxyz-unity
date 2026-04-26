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

            List<int> faceTriangles = TriangulateEarClipping(points);

            AddBottomFace(vertices, triangles,faceTriangles, points, geometry.bottom);
            AddTopFace(vertices, triangles, faceTriangles, points, geometry.top);
            AddSideFaces(vertices, triangles, points, geometry.bottom, geometry.top);

            var mesh = new Mesh();
            mesh.name = "BuildingExtrusionMesh";
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void AddBottomFace(List<Vector3> vertices, List<int> triangles, List<int> faceTriangles, List<Vector2> points, float y)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(new Vector3(p.x, y, p.y));

            for (int i = 0; i < faceTriangles.Count; i += 3)
            {
                triangles.Add(start + faceTriangles[i + 0]);
                triangles.Add(start + faceTriangles[i + 1]);
                triangles.Add(start + faceTriangles[i + 2]);
            }
        }

        private static void AddTopFace(List<Vector3> vertices, List<int> triangles, List<int> faceTriangles, List<Vector2> points, float y)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(new Vector3(p.x, y, p.y));

            for (int i = 0; i < faceTriangles.Count; i += 3)
            {
                triangles.Add(start + faceTriangles[i + 0]);
                triangles.Add(start + faceTriangles[i + 2]);
                triangles.Add(start + faceTriangles[i + 1]);
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

        private static List<int> TriangulateEarClipping(List<Vector2> points)
        {
            var indices = new List<int>();
            var remaining = new List<int>();
            for (int i = 0; i < points.Count; i++)
                remaining.Add(i);
            int guard = 0;
            while (remaining.Count > 3 && guard < 10000)
            {
                guard++;
                bool earFound = false;
                for (int i = 0; i < remaining.Count; i++)
                {
                    int prevIndex = remaining[(i - 1 + remaining.Count) % remaining.Count];
                    int currentIndex = remaining[i];
                    int nextIndex = remaining[(i + 1) % remaining.Count];
                    Vector2 prev = points[prevIndex];
                    Vector2 current = points[currentIndex];
                    Vector2 next = points[nextIndex];
                    if (!IsConvex(prev, current, next))
                        continue;
                    if (ContainsAnyPoint(points, remaining, prevIndex, currentIndex, nextIndex))
                        continue;
                    indices.Add(prevIndex);
                    indices.Add(currentIndex);
                    indices.Add(nextIndex);
                    remaining.RemoveAt(i);
                    earFound = true;
                    break;
                }
                if (!earFound)
                {
                    Debug.LogError("Failed to triangulate polygon. Check contour order or self-intersections.");
                    break;
                }
            }
            if (remaining.Count == 3)
            {
                indices.Add(remaining[0]);
                indices.Add(remaining[1]);
                indices.Add(remaining[2]);
            }
            return indices;
        }

        private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            float cross = ab.x * bc.y - ab.y * bc.x;
            // После NormalizeWinding у нас ожидается CCW-контур.
            return cross > 0f;
        }

        private static bool ContainsAnyPoint(
            List<Vector2> points,
            List<int> remaining,
            int aIndex,
            int bIndex,
            int cIndex)
        {
            Vector2 a = points[aIndex];
            Vector2 b = points[bIndex];
            Vector2 c = points[cIndex];
            foreach (int index in remaining)
            {
                if (index == aIndex || index == bIndex || index == cIndex)
                    continue;
                if (IsPointInsideTriangle(points[index], a, b, c))
                    return true;
            }
            return false;
        }


        private static bool IsPointInsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float area = Cross(b - a, c - a);
            float area1 = Cross(b - a, p - a);
            float area2 = Cross(c - b, p - b);
            float area3 = Cross(a - c, p - c);
            bool hasNegative = area1 < 0f || area2 < 0f || area3 < 0f;
            bool hasPositive = area1 > 0f || area2 > 0f || area3 > 0f;
            return !(hasNegative && hasPositive);
        }

        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static List<Vector2> NormalizeWinding(List<Vector2> points)
        {
            float signedArea = GetSignedArea(points);
            Debug.Log($"[Mesh] Signed area: {signedArea}");

            if (signedArea < 0f)
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