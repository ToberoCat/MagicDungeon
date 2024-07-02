using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spells
{
    public static class SpellRecognizer
    {
        public static RecognizedSpell[] RecognizeSpells(List<Spell> spells, List<Vector2> drawnPath)
        {
            var recognizedSpells = new RecognizedSpell[spells.Count];
            for (var i = 0; i < spells.Count; i++)
            {
                var similarity = CalculateSimilarity(drawnPath.ToArray(), spells[i].path);
                recognizedSpells[i] = new RecognizedSpell { Spell = spells[i], Similarity = similarity };
            }

            return recognizedSpells.OrderByDescending(rs => rs.Similarity).ToArray();
        }

        public static List<Vector2> NormalizeDrawing(List<Vector2> points)
        {
            if (points == null || points.Count == 0)
                return new List<Vector2>();

            var minX = points.Min(p => p.x);
            var minY = points.Min(p => p.y);
            var maxX = points.Max(p => p.x);
            var maxY = points.Max(p => p.y);

            var rangeX = maxX - minX;
            var rangeY = maxY - minY;

            var normalizedPoints = points.Select(p =>
            {
                var normalizedX = (p.x - minX) / rangeX;
                var normalizedY = (p.y - minY) / rangeY;
                return new Vector2(normalizedX, normalizedY);
            }).ToList();

            return normalizedPoints;
        }

        private static float CalculateSimilarity(Vector2[] drawing1, Vector2[] drawing2)
        {
            var hausdorff1 = HausdorffDistance(drawing1, drawing2);
            var hausdorff2 = HausdorffDistance(drawing2, drawing1);

            var coverage1 = CoverageRatio(drawing1, drawing2);
            var coverage2 = CoverageRatio(drawing2, drawing1);

            return (coverage1 + coverage2) / 2.0f * (1.0f / (1.0f + Mathf.Max(hausdorff1, hausdorff2)));
        }

        private static float HausdorffDistance(Vector2[] shape1, Vector2[] shape2) => shape1
            .Select(point1 => shape2
                .Select(point2 => Vector2.Distance(point1, point2))
                .Prepend(float.MaxValue)
                .Min())
            .Prepend(0.0f)
            .Max();

        private static float CoverageRatio(Vector2[] shape1, Vector2[] shape2)
        {
            var visitedCount = shape1.Count(point1 =>
                shape2.Select(point2 => Vector2.Distance(point1, point2)).Any(distance => distance < 0.1f));
            return (float)visitedCount / shape1.Length;
        }


        public static List<Vector2> ConvertDrawing(List<Vector2> points)
        {
            var simplifiedPath = SimplifyDrawing(points);
            return NormalizeDrawing(simplifiedPath);
        }

        public static List<Vector2> SimplifyDrawing(List<Vector2> points, int fixedLength = 32,
            float snapDistanceThreshold = 0.05f)
        {
            if (points == null || points.Count < 2 || points.Count == fixedLength)
                return points;

            var totalLength = CalculateTotalLength(points);
            var segmentLength = totalLength / (fixedLength - 1);
            var simplifiedPoints = SimplifyRdp(points, segmentLength / 2f);
            OptimizePoints(ref simplifiedPoints, segmentLength / 2f);
            if (simplifiedPoints.Count > fixedLength)
            {
                var simplificationIterations = 1;
                while (simplifiedPoints.Count > fixedLength)
                {
                    OptimizePoints(ref simplifiedPoints, snapDistanceThreshold * simplificationIterations++);
                    simplifiedPoints = SimplifyRdp(points, segmentLength / 2f);
                }
            }

            if (simplifiedPoints.Count > fixedLength)
                throw new System.Exception("Failed to simplify the drawing to the desired number of points.");

            return simplifiedPoints.Count == fixedLength
                ? simplifiedPoints
                : IncreaseResolution(simplifiedPoints, fixedLength);
        }

        private static List<Vector2> IncreaseResolution(List<Vector2> points, int fixedLength)
        {
            var cumulativeDistances = CalculateCumulativeDistances(points);
            return InterpolatePoints(points, cumulativeDistances, fixedLength);
        }

        private static List<Vector2> SimplifyRdp(List<Vector2> points, float epsilon)
        {
            if (points.Count < 3)
                return points;

            var simplifiedPoints = new List<Vector2> { points[0] };
            SimplifySegment(points, 0, points.Count - 1, epsilon, simplifiedPoints);
            simplifiedPoints.Add(points[^1]);

            return simplifiedPoints;
        }

        private static void SimplifySegment(List<Vector2> points, int start, int end, float epsilon,
            List<Vector2> simplifiedPoints)
        {
            while (true)
            {
                float maxDistance = 0;
                var farthestIndex = 0;

                for (var i = start + 1; i < end; i++)
                {
                    var distance = PerpendicularDistance(points[i], points[start], points[end]);
                    if (!(distance > maxDistance)) continue;
                    maxDistance = distance;
                    farthestIndex = i;
                }

                if (maxDistance > epsilon)
                {
                    SimplifySegment(points, start, farthestIndex, epsilon, simplifiedPoints);
                    start = farthestIndex;
                    continue;
                }

                simplifiedPoints.Add(points[end]);
                break;
            }
        }

        private static float PerpendicularDistance(Vector2 point, Vector2 start, Vector2 end)
        {
            var segmentLength = Vector2.Distance(start, end);
            if (segmentLength == 0)
                return Vector2.Distance(point, start);

            var t = Mathf.Clamp01(Vector2.Dot(point - start, end - start) / (segmentLength * segmentLength));
            var projection = start + t * (end - start);
            return Vector2.Distance(point, projection);
        }

        private static float CalculateTotalLength(List<Vector2> points)
        {
            float totalLength = 0;
            for (var i = 1; i < points.Count; i++)
                totalLength += Vector2.Distance(points[i - 1], points[i]);

            return totalLength;
        }

        private static float[] CalculateCumulativeDistances(List<Vector2> points)
        {
            var cumulativeDistances = new float[points.Count];
            cumulativeDistances[0] = 0;
            for (var i = 1; i < points.Count; i++)
                cumulativeDistances[i] = cumulativeDistances[i - 1] + Vector2.Distance(points[i - 1], points[i]);

            return cumulativeDistances;
        }

        private static List<Vector2> InterpolatePoints(List<Vector2> points, float[] cumulativeDistances,
            int interpolateToLength = 32)
        {
            var interpolatedPoints = new List<Vector2> { points[0] };
            var targetLength = cumulativeDistances[^1];
            var targetIndex = 1;
            for (var i = 1; i < interpolateToLength - 1; i++)
            {
                var targetCumulativeDistance = i * targetLength / (interpolateToLength - 1);
                while (cumulativeDistances[targetIndex] < targetCumulativeDistance)
                    targetIndex++;

                var t = (targetCumulativeDistance - cumulativeDistances[targetIndex - 1]) /
                        (cumulativeDistances[targetIndex] - cumulativeDistances[targetIndex - 1]);
                var interpolatedPoint = Vector2.Lerp(points[targetIndex - 1], points[targetIndex], t);
                interpolatedPoints.Add(interpolatedPoint);
            }

            interpolatedPoints.Add(points[^1]);
            return interpolatedPoints;
        }

        private static void OptimizePoints(ref List<Vector2> points, float snapDistanceThreshold = 0.05f)
        {
            for (var i = 1; i < points.Count - 1; i++)
            {
                var prevPoint = points[i - 1];
                var currPoint = points[i];
                var nextPoint = points[i + 1];

                if (Vector2.Distance(currPoint, nextPoint) < snapDistanceThreshold)
                {
                    points[i] = Vector2.Lerp(currPoint, nextPoint, 0.5f);
                    points.RemoveAt(i + 1);
                    i--;
                }

                else if (Vector2.Distance(prevPoint, currPoint) < snapDistanceThreshold)
                {
                    points[i] = Vector2.Lerp(prevPoint, currPoint, 0.5f);
                    points.RemoveAt(i - 1);
                    i--;
                }
            }
        }
    }

    public struct RecognizedSpell
    {
        public Spell Spell;
        public float Similarity;
    }
}