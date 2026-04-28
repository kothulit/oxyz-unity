using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public static class SpatialRegionSplitter
    {
        private const float Epsilon = 0.001f;

        public static SpatialPlanRegion[] SelectRegionsContainingPoint(
            SpatialPlanRegion initialRegion,
            SpatialBoundary[] boundaries,
            Vector2 point)
        {
            if (!IsValidPolygon(initialRegion.points))
                return System.Array.Empty<SpatialPlanRegion>();

            var selectedRegions = new List<SpatialPlanRegion> { initialRegion };

            if (boundaries == null || boundaries.Length == 0)
                return selectedRegions.ToArray();

            for (int i = 0; i < boundaries.Length; i++)
            {
                var nextRegions = new List<SpatialPlanRegion>();

                for (int j = 0; j < selectedRegions.Count; j++)
                {
                    SpatialPlanRegion[] splitRegions = SelectAfterBoundary(selectedRegions[j], boundaries[i], point);
                    nextRegions.AddRange(splitRegions);
                }

                if (nextRegions.Count > 0)
                    selectedRegions = nextRegions;
            }

            return selectedRegions.ToArray();
        }

        public static bool ContainsPoint(Vector2[] polygon, Vector2 point)
        {
            if (!IsValidPolygon(polygon))
                return false;

            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[j];

                if (IsPointOnSegment(point, a, b))
                    return true;

                bool intersects = (a.y > point.y) != (b.y > point.y)
                                  && point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x;
                if (intersects)
                    inside = !inside;
            }

            return inside;
        }

        private static SpatialPlanRegion[] SelectAfterBoundary(
            SpatialPlanRegion region,
            SpatialBoundary boundary,
            Vector2 point)
        {
            if (boundary.points == null)
                return new[] { region };

            switch (boundary.shape)
            {
                case SpatialBoundaryShape.Loop:
                    return SelectAfterLoop(region, boundary.points, point);
                case SpatialBoundaryShape.Beam:
                    return SelectAfterLine(region, boundary.points, point, requireSegmentCrossing: false);
                case SpatialBoundaryShape.Piece:
                    return SelectAfterLine(region, boundary.points, point, requireSegmentCrossing: true);
                default:
                    return new[] { region };
            }
        }

        private static SpatialPlanRegion[] SelectAfterLoop(SpatialPlanRegion region, Vector2[] loop, Vector2 point)
        {
            if (!IsValidPolygon(loop))
                return new[] { CopyRegion(region) };

            if (!ContainsPoint(region.points, point))
                return System.Array.Empty<SpatialPlanRegion>();

            if (ContainsPoint(loop, point))
            {
                return new[]
                {
                    new SpatialPlanRegion { points = CopyPoints(loop) }
                };
            }

            if (TryCreateOutsideRegionBySharedEdge(region.points, loop, point, out SpatialPlanRegion outsideRegion))
                return new[] { outsideRegion };

            if (!IsPolygonInsideRegion(region.points, loop))
                return new[] { CopyRegion(region) };

            return new[]
            {
                new SpatialPlanRegion
                {
                    points = CopyPoints(region.points),
                    holes = AppendHole(region.holes, loop)
                }
            };
        }

        private static bool IsPolygonInsideRegion(Vector2[] region, Vector2[] polygon)
        {
            for (int i = 0; i < polygon.Length; i++)
            {
                if (!ContainsPoint(region, polygon[i]))
                    return false;
            }

            return true;
        }

        private static bool TryCreateOutsideRegionBySharedEdge(
            Vector2[] region,
            Vector2[] loop,
            Vector2 point,
            out SpatialPlanRegion outsideRegion)
        {
            outsideRegion = default;

            for (int regionIndex = 0; regionIndex < region.Length; regionIndex++)
            {
                int nextRegionIndex = (regionIndex + 1) % region.Length;
                Vector2 regionStart = region[regionIndex];
                Vector2 regionEnd = region[nextRegionIndex];

                for (int loopIndex = 0; loopIndex < loop.Length; loopIndex++)
                {
                    int nextLoopIndex = (loopIndex + 1) % loop.Length;
                    Vector2 loopStart = loop[loopIndex];
                    Vector2 loopEnd = loop[nextLoopIndex];

                    if (!AreClose(regionStart, loopStart) || !AreClose(regionEnd, loopEnd))
                        continue;

                    Vector2[] outside = BuildOutsideRegion(region, regionIndex, nextRegionIndex, loop, loopIndex, nextLoopIndex);
                    if (IsValidPolygon(outside) && ContainsPoint(outside, point))
                    {
                        outsideRegion = new SpatialPlanRegion { points = outside };
                        return true;
                    }
                }
            }

            return false;
        }

        private static Vector2[] BuildOutsideRegion(
            Vector2[] region,
            int sharedRegionStart,
            int sharedRegionEnd,
            Vector2[] loop,
            int sharedLoopStart,
            int sharedLoopEnd)
        {
            var outside = new List<Vector2>();

            AddUnique(outside, region[sharedRegionEnd]);
            int regionIndex = (sharedRegionEnd + 1) % region.Length;
            while (regionIndex != sharedRegionStart)
            {
                AddUnique(outside, region[regionIndex]);
                regionIndex = (regionIndex + 1) % region.Length;
            }
            AddUnique(outside, region[sharedRegionStart]);

            int loopIndex = (sharedLoopStart - 1 + loop.Length) % loop.Length;
            while (loopIndex != sharedLoopEnd)
            {
                AddUnique(outside, loop[loopIndex]);
                loopIndex = (loopIndex - 1 + loop.Length) % loop.Length;
            }
            AddUnique(outside, loop[sharedLoopEnd]);

            return RemoveDuplicateClosingPoint(outside).ToArray();
        }

        private static SpatialPlanRegion[] SelectAfterLine(
            SpatialPlanRegion region,
            Vector2[] path,
            Vector2 point,
            bool requireSegmentCrossing)
        {
            if (!ContainsPoint(region.points, point) || path.Length < 2)
                return System.Array.Empty<SpatialPlanRegion>();

            Vector2 start = path[0];
            Vector2 end = path[path.Length - 1];
            if ((end - start).sqrMagnitude <= Epsilon * Epsilon)
                return new[] { region };

            if (requireSegmentCrossing && CountSegmentBoundaryIntersections(region.points, start, end) < 2)
                return new[] { region };

            SpatialPlanRegion[] splitRegions = SplitByLine(region.points, start, end);
            if (splitRegions.Length == 0)
                return new[] { region };

            var selected = new List<SpatialPlanRegion>();
            for (int i = 0; i < splitRegions.Length; i++)
            {
                if (ContainsPoint(splitRegions[i].points, point))
                    selected.Add(splitRegions[i]);
            }

            return selected.Count > 0 ? selected.ToArray() : new[] { region };
        }

        private static SpatialPlanRegion[] SplitByLine(Vector2[] polygon, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2[] left = ClipByLineSide(polygon, lineStart, lineEnd, keepLeft: true);
            Vector2[] right = ClipByLineSide(polygon, lineStart, lineEnd, keepLeft: false);

            var result = new List<SpatialPlanRegion>();
            if (IsValidPolygon(left))
                result.Add(new SpatialPlanRegion { points = left });
            if (IsValidPolygon(right))
                result.Add(new SpatialPlanRegion { points = right });

            return result.Count == 2 ? result.ToArray() : System.Array.Empty<SpatialPlanRegion>();
        }

        private static Vector2[] ClipByLineSide(Vector2[] polygon, Vector2 lineStart, Vector2 lineEnd, bool keepLeft)
        {
            var output = new List<Vector2>();
            Vector2 previous = polygon[polygon.Length - 1];
            float previousSide = SignedDistance(lineStart, lineEnd, previous);
            bool previousInside = IsInside(previousSide, keepLeft);

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 current = polygon[i];
                float currentSide = SignedDistance(lineStart, lineEnd, current);
                bool currentInside = IsInside(currentSide, keepLeft);

                if (currentInside != previousInside)
                {
                    if (TryLineIntersection(previous, current, lineStart, lineEnd, out Vector2 intersection))
                        AddUnique(output, intersection);
                }

                if (currentInside)
                    AddUnique(output, current);

                previous = current;
                previousSide = currentSide;
                previousInside = currentInside;
            }

            return RemoveDuplicateClosingPoint(output).ToArray();
        }

        private static bool IsInside(float side, bool keepLeft)
        {
            return keepLeft ? side >= -Epsilon : side <= Epsilon;
        }

        private static int CountSegmentBoundaryIntersections(Vector2[] polygon, Vector2 start, Vector2 end)
        {
            int count = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[(i + 1) % polygon.Length];
                if (TrySegmentIntersection(start, end, a, b, out _))
                    count++;
            }

            return count;
        }

        private static bool TrySegmentIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 r = b - a;
            Vector2 s = d - c;
            float denominator = Cross(r, s);
            if (Mathf.Abs(denominator) <= Epsilon)
                return false;

            float t = Cross(c - a, s) / denominator;
            float u = Cross(c - a, r) / denominator;
            if (t < -Epsilon || t > 1f + Epsilon || u < -Epsilon || u > 1f + Epsilon)
                return false;

            intersection = a + t * r;
            return true;
        }

        private static bool TryLineIntersection(Vector2 a, Vector2 b, Vector2 lineStart, Vector2 lineEnd, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 segment = b - a;
            Vector2 line = lineEnd - lineStart;
            float denominator = Cross(segment, line);
            if (Mathf.Abs(denominator) <= Epsilon)
                return false;

            float t = Cross(lineStart - a, line) / denominator;
            intersection = a + t * segment;
            return true;
        }

        private static bool IsPointOnSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            float cross = Mathf.Abs(Cross(b - a, point - a));
            if (cross > Epsilon)
                return false;

            float dot = Vector2.Dot(point - a, point - b);
            return dot <= Epsilon;
        }

        private static bool AreClose(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude <= Epsilon * Epsilon;
        }

        private static float SignedDistance(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
        {
            return Cross(lineEnd - lineStart, point - lineStart);
        }

        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static bool IsValidPolygon(Vector2[] polygon)
        {
            return polygon != null && polygon.Length >= 3 && Mathf.Abs(GetSignedArea(polygon)) > Epsilon;
        }

        private static float GetSignedArea(Vector2[] polygon)
        {
            float area = 0f;
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 current = polygon[i];
                Vector2 next = polygon[(i + 1) % polygon.Length];
                area += current.x * next.y - next.x * current.y;
            }

            return area * 0.5f;
        }

        private static Vector2[] CopyPoints(Vector2[] source)
        {
            var copy = new Vector2[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = source[i];
            }

            return copy;
        }

        private static SpatialPlanRegion CopyRegion(SpatialPlanRegion source)
        {
            return new SpatialPlanRegion
            {
                points = CopyPoints(source.points),
                holes = CopyHoles(source.holes)
            };
        }

        private static Vector2[][] AppendHole(Vector2[][] existingHoles, Vector2[] hole)
        {
            int existingCount = existingHoles?.Length ?? 0;
            Vector2[][] result = new Vector2[existingCount + 1][];

            for (int i = 0; i < existingCount; i++)
            {
                result[i] = CopyPoints(existingHoles[i]);
            }

            result[existingCount] = CopyPoints(hole);
            return result;
        }

        private static Vector2[][] CopyHoles(Vector2[][] source)
        {
            if (source == null || source.Length == 0)
                return null;

            Vector2[][] copy = new Vector2[source.Length][];
            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = CopyPoints(source[i]);
            }

            return copy;
        }

        private static void AddUnique(List<Vector2> points, Vector2 point)
        {
            if (points.Count > 0 && (points[points.Count - 1] - point).sqrMagnitude <= Epsilon * Epsilon)
                return;

            points.Add(point);
        }

        private static List<Vector2> RemoveDuplicateClosingPoint(List<Vector2> points)
        {
            if (points.Count > 1 && (points[0] - points[points.Count - 1]).sqrMagnitude <= Epsilon * Epsilon)
                points.RemoveAt(points.Count - 1);

            return points;
        }
    }
}
