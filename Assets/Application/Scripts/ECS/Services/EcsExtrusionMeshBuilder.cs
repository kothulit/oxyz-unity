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

            return Build(new SpatialVolume
            {
                bottom = geometry.bottom,
                top = geometry.top,
                points = geometry.points
            });
        }

        public static Mesh Build(SpatialVolume geometry)
        {
            if (geometry.points == null || geometry.points.Length < 3)
                return null;

            List<Vector2> points = NormalizeWinding(new List<Vector2>(geometry.points));
            List<List<Vector2>> holes = NormalizeHoles(geometry.holes);

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            if (holes.Count > 0)
            {
                AddGridCapFace(vertices, triangles, points, holes, geometry.bottom, isTop: false);
                AddGridCapFace(vertices, triangles, points, holes, geometry.top, isTop: true);
            }
            else
            {
                List<int> faceTriangles = TriangulateEarClipping(points);
                AddBottomFace(vertices, triangles, faceTriangles, points, geometry.bottom);
                AddTopFace(vertices, triangles, faceTriangles, points, geometry.top);
            }

            AddSideFaces(vertices, triangles, points, geometry.bottom, geometry.top, reverseWinding: false);
            for (int i = 0; i < holes.Count; i++)
            {
                AddSideFaces(vertices, triangles, holes[i], geometry.bottom, geometry.top, reverseWinding: false);
            }

            var mesh = new Mesh();
            mesh.name = "BuildingExtrusionMesh";
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void AddBottomFace(List<Vector3> vertices, List<int> triangles, List<int> faceTriangles, List<Vector2> points, float z)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(ToUnityPoint(p, z));

            for (int i = 0; i < faceTriangles.Count; i += 3)
            {
                triangles.Add(start + faceTriangles[i + 0]);
                triangles.Add(start + faceTriangles[i + 1]);
                triangles.Add(start + faceTriangles[i + 2]);
            }
        }

        private static void AddGridCapFace(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> outer,
            List<List<Vector2>> holes,
            float z,
            bool isTop)
        {
            List<float> xs = CollectSortedCoordinates(outer, holes, useX: true);
            List<float> ys = CollectSortedCoordinates(outer, holes, useX: false);

            for (int xIndex = 0; xIndex < xs.Count - 1; xIndex++)
            {
                for (int yIndex = 0; yIndex < ys.Count - 1; yIndex++)
                {
                    float x0 = xs[xIndex];
                    float x1 = xs[xIndex + 1];
                    float y0 = ys[yIndex];
                    float y1 = ys[yIndex + 1];

                    if (Mathf.Abs(x1 - x0) <= Mathf.Epsilon || Mathf.Abs(y1 - y0) <= Mathf.Epsilon)
                        continue;

                    var center = new Vector2((x0 + x1) * 0.5f, (y0 + y1) * 0.5f);
                    if (!IsPointInsidePolygon(center, outer))
                        continue;
                    if (IsInsideAnyHole(center, holes))
                        continue;

                    int start = vertices.Count;
                    vertices.Add(ToUnityPoint(new Vector2(x0, y0), z));
                    vertices.Add(ToUnityPoint(new Vector2(x1, y0), z));
                    vertices.Add(ToUnityPoint(new Vector2(x1, y1), z));
                    vertices.Add(ToUnityPoint(new Vector2(x0, y1), z));

                    if (isTop)
                    {
                        triangles.Add(start + 0);
                        triangles.Add(start + 2);
                        triangles.Add(start + 1);

                        triangles.Add(start + 0);
                        triangles.Add(start + 3);
                        triangles.Add(start + 2);
                    }
                    else
                    {
                        triangles.Add(start + 0);
                        triangles.Add(start + 1);
                        triangles.Add(start + 2);

                        triangles.Add(start + 0);
                        triangles.Add(start + 2);
                        triangles.Add(start + 3);
                    }
                }
            }
        }

        private static List<float> CollectSortedCoordinates(List<Vector2> outer, List<List<Vector2>> holes, bool useX)
        {
            var coordinates = new List<float>();
            AddCoordinates(coordinates, outer, useX);

            for (int i = 0; i < holes.Count; i++)
            {
                AddCoordinates(coordinates, holes[i], useX);
            }

            coordinates.Sort();

            for (int i = coordinates.Count - 2; i >= 0; i--)
            {
                if (Mathf.Abs(coordinates[i + 1] - coordinates[i]) <= 0.0001f)
                    coordinates.RemoveAt(i + 1);
            }

            return coordinates;
        }

        private static void AddCoordinates(List<float> coordinates, List<Vector2> points, bool useX)
        {
            for (int i = 0; i < points.Count; i++)
            {
                coordinates.Add(useX ? points[i].x : points[i].y);
            }
        }

        private static bool IsInsideAnyHole(Vector2 point, List<List<Vector2>> holes)
        {
            for (int i = 0; i < holes.Count; i++)
            {
                if (IsPointInsidePolygon(point, holes[i]))
                    return true;
            }

            return false;
        }

        private static void AddTopFace(List<Vector3> vertices, List<int> triangles, List<int> faceTriangles, List<Vector2> points, float z)
        {
            int start = vertices.Count;

            foreach (var p in points)
                vertices.Add(ToUnityPoint(p, z));

            for (int i = 0; i < faceTriangles.Count; i += 3)
            {
                triangles.Add(start + faceTriangles[i + 0]);
                triangles.Add(start + faceTriangles[i + 2]);
                triangles.Add(start + faceTriangles[i + 1]);
            }
        }

        private static void AddSideFaces(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> points,
            float bottom,
            float top,
            bool reverseWinding)
        {
            for (int i = 0; i < points.Count; i++)
            {
                int next = (i + 1) % points.Count;

                Vector2 p0 = points[i];
                Vector2 p1 = points[next];

                int start = vertices.Count;

                vertices.Add(ToUnityPoint(p0, bottom));
                vertices.Add(ToUnityPoint(p1, bottom));
                vertices.Add(ToUnityPoint(p1, top));
                vertices.Add(ToUnityPoint(p0, top));

                if (reverseWinding)
                {
                    triangles.Add(start + 0);
                    triangles.Add(start + 1);
                    triangles.Add(start + 2);

                    triangles.Add(start + 0);
                    triangles.Add(start + 2);
                    triangles.Add(start + 3);
                }
                else
                {
                    triangles.Add(start + 0);
                    triangles.Add(start + 2);
                    triangles.Add(start + 1);

                    triangles.Add(start + 0);
                    triangles.Add(start + 3);
                    triangles.Add(start + 2);
                }
            }
        }

        private static List<Vector2> BuildFaceContour(List<Vector2> outer, List<List<Vector2>> holes)
        {
            var contour = new List<Vector2>(outer);

            for (int i = 0; i < holes.Count; i++)
            {
                contour = BridgeHole(contour, holes[i]);
            }

            return contour;
        }

        private static List<Vector2> BridgeHole(List<Vector2> contour, List<Vector2> hole)
        {
            int holeIndex = GetRightmostPointIndex(hole);
            Vector2 holePoint = hole[holeIndex];

            if (!TryFindRightRayIntersection(contour, holePoint, out int contourEdgeStartIndex, out Vector2 bridgePoint))
            {
                contourEdgeStartIndex = GetNearestVisiblePointIndex(contour, holePoint);
                bridgePoint = contour[contourEdgeStartIndex];
            }

            var bridged = new List<Vector2>();
            for (int i = 0; i <= contourEdgeStartIndex; i++)
            {
                bridged.Add(contour[i]);
            }

            bridged.Add(bridgePoint);
            bridged.Add(holePoint);
            for (int i = 1; i < hole.Count; i++)
            {
                int index = (holeIndex + i) % hole.Count;
                bridged.Add(hole[index]);
            }
            bridged.Add(holePoint);
            bridged.Add(bridgePoint);

            for (int i = contourEdgeStartIndex + 1; i < contour.Count; i++)
            {
                bridged.Add(contour[i]);
            }

            return bridged;
        }

        private static bool TryFindRightRayIntersection(
            List<Vector2> contour,
            Vector2 point,
            out int edgeStartIndex,
            out Vector2 intersection)
        {
            edgeStartIndex = -1;
            intersection = Vector2.zero;
            float bestX = float.PositiveInfinity;

            for (int i = 0; i < contour.Count; i++)
            {
                int next = (i + 1) % contour.Count;
                Vector2 a = contour[i];
                Vector2 b = contour[next];

                if (!TryIntersectHorizontalRay(point, a, b, out Vector2 candidate))
                    continue;
                if (candidate.x <= point.x)
                    continue;
                if (candidate.x >= bestX)
                    continue;

                bestX = candidate.x;
                edgeStartIndex = i;
                intersection = candidate;
            }

            return edgeStartIndex >= 0;
        }

        private static bool TryIntersectHorizontalRay(Vector2 origin, Vector2 a, Vector2 b, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            if (Mathf.Abs(a.y - b.y) <= Mathf.Epsilon)
                return false;
            if (origin.y < Mathf.Min(a.y, b.y) || origin.y > Mathf.Max(a.y, b.y))
                return false;

            float t = (origin.y - a.y) / (b.y - a.y);
            if (t < 0f || t > 1f)
                return false;

            float x = Mathf.Lerp(a.x, b.x, t);
            intersection = new Vector2(x, origin.y);
            return true;
        }

        private static int GetRightmostPointIndex(List<Vector2> points)
        {
            int result = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].x > points[result].x || Mathf.Approximately(points[i].x, points[result].x) && points[i].y < points[result].y)
                    result = i;
            }

            return result;
        }

        private static int GetNearestVisiblePointIndex(List<Vector2> contour, Vector2 point)
        {
            int result = 0;
            float bestDistance = float.PositiveInfinity;

            for (int i = 0; i < contour.Count; i++)
            {
                float distance = (contour[i] - point).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    result = i;
                }
            }

            return result;
        }

        private static Vector3 ToUnityPoint(Vector2 planPoint, float z)
        {
            return new Vector3(planPoint.x, z, planPoint.y);
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
            // ˜˜˜˜˜ NormalizeWinding ˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜ CCW-˜˜˜˜˜˜.
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
                if (IsSamePoint(points[index], a) || IsSamePoint(points[index], b) || IsSamePoint(points[index], c))
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

        private static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[j];

                bool intersects = (a.y > point.y) != (b.y > point.y)
                                  && point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x;
                if (intersects)
                    inside = !inside;
            }

            return inside;
        }

        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static bool IsSamePoint(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude <= 0.000001f;
        }

        private static List<Vector2> NormalizeWinding(List<Vector2> points)
        {
            float signedArea = GetSignedArea(points);
            Debug.Log($"[Mesh] Signed area: {signedArea}");

            if (signedArea < 0f)
                points.Reverse();

            return points;
        }

        private static List<List<Vector2>> NormalizeHoles(Vector2[][] holes)
        {
            var result = new List<List<Vector2>>();
            if (holes == null)
                return result;

            for (int i = 0; i < holes.Length; i++)
            {
                if (holes[i] == null || holes[i].Length < 3)
                    continue;

                var hole = new List<Vector2>(holes[i]);
                if (GetSignedArea(hole) > 0f)
                    hole.Reverse();

                result.Add(hole);
            }

            return result;
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