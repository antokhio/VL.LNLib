using System.Runtime.CompilerServices;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    public static class NurbsCurveTessellator
    {
        /// <summary>
        /// Tessellates a NURBS curve into a polyline using adaptive recursive subdivision.
        /// </summary>
        /// <typeparam name="T">The vector type (Vector2 or Vector3).</typeparam>
        /// <param name="curve">The curve to tessellate.</param>
        /// <param name="tolerance">The maximum deviation allowed between the curve and the linear segments.</param>
        /// <param name="maxDepth">Maximum recursion depth to prevent infinite loops (performance safety).</param>
        /// <returns>A list of points representing the tessellated curve.</returns>
        public static IReadOnlyList<T> Tessellate<T>(
            NurbsCurve<T> curve,
            float tolerance = 0.01f,
            int maxDepth = 10
        )
            where T : struct
        {
            // Safety checks
            if (
                curve.NativeCurve.control_points == IntPtr.Zero
                || curve.Knots == null
                || curve.Knots.Count == 0
            )
            {
                return Array.Empty<T>();
            }

            // Valid domain of the curve
            float tStart = curve.Knots[0];
            float tEnd = curve.Knots[curve.Knots.Count - 1];

            // Evaluate endpoints
            T pStart = curve.GetPointOnCurve(tStart);
            T pEnd = curve.GetPointOnCurve(tEnd);

            var points = new List<T>();
            points.Add(pStart);

            // Start recursive subdivision
            RecursiveTessellate(curve, points, tStart, tEnd, pStart, pEnd, tolerance, maxDepth);

            // Add the final point
            points.Add(pEnd);

            return points;
        }

        private static void RecursiveTessellate<T>(
            NurbsCurve<T> curve,
            List<T> points,
            float t0,
            float t1,
            T p0,
            T p1,
            float tolerance,
            int depth
        )
            where T : struct
        {
            // Stop if max depth reached
            if (depth <= 0)
                return;

            // Calculate mid parameter and point
            float tMid = (t0 + t1) * 0.5f;
            T pMid = curve.GetPointOnCurve(tMid);

            // Check if the segment (p0 -> pMid -> p1) is flat enough
            if (IsFlatEnough(p0, p1, pMid, tolerance))
            {
                // The segment is linear enough within tolerance.
                // We do NOT add pMid here to keep the polyline minimal.
                // Recursion stops for this branch.
                return;
            }
            else
            {
                // Not flat enough, subdivide further
                RecursiveTessellate(curve, points, t0, tMid, p0, pMid, tolerance, depth - 1);

                // Add the mid point to the list (in order)
                points.Add(pMid);

                RecursiveTessellate(curve, points, tMid, t1, pMid, p1, tolerance, depth - 1);
            }
        }

        /// <summary>
        /// Checks if the triangle formed by p0, p1, and pMid has a height smaller than tolerance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFlatEnough<T>(T p0, T p1, T pMid, float tolerance)
            where T : struct
        {
            Vector3 v0,
                v1,
                vMid;

            // Unified handling by converting Vector2 to Vector3 (z=0)
            if (typeof(T) == typeof(Vector2))
            {
                var v2_0 = Unsafe.As<T, Vector2>(ref p0);
                var v2_1 = Unsafe.As<T, Vector2>(ref p1);
                var v2_Mid = Unsafe.As<T, Vector2>(ref pMid);
                v0 = new Vector3(v2_0, 0);
                v1 = new Vector3(v2_1, 0);
                vMid = new Vector3(v2_Mid, 0);
            }
            else
            {
                v0 = Unsafe.As<T, Vector3>(ref p0);
                v1 = Unsafe.As<T, Vector3>(ref p1);
                vMid = Unsafe.As<T, Vector3>(ref pMid);
            }

            // Direction of the chord
            Vector3 chord = v1 - v0;
            float chordLenSq = chord.LengthSquared();

            // If start and end are practically the same point, check simple distance
            if (chordLenSq < 1e-6f)
            {
                return Vector3.DistanceSquared(v0, vMid) < tolerance * tolerance;
            }

            // Distance from pMid to the line passing through p0-p1.
            // Formula: Height = |Cross(Chord, pMid-p0)| / |Chord|
            // We check: Height^2 < Tolerance^2

            Vector3 edge = vMid - v0;
            Vector3 cross = Vector3.Cross(chord, edge);

            // (Height^2) = Cross^2 / Chord^2
            // Check: Cross^2 / Chord^2 < Tol^2
            // Avoid division: Cross^2 < Tol^2 * Chord^2

            return cross.LengthSquared() < (tolerance * tolerance * chordLenSq);
        }
    }
}
