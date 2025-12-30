using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    public record RationalBezierCurve2D : BezierCurve<Vector3>
    {
        public RationalBezierCurve2D(int degree, IReadOnlyList<Vector3> controlPoints)
            : base(degree, controlPoints) { }

        public override Vector3 GetPointOnCurve(float t)
        {
            // Input ControlPoints are: X, Y, Weight

            int count = ControlPoints.Count;
            var pool = ArrayPool<XYZW>.Shared;
            XYZW[] nativePoints = pool.Rent(count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var v = ControlPoints[i];
                    double w = v.Z;

                    // Convert Euclidean Control Point to Homogeneous Control Point
                    // P_hom = (x*w, y*w, z*w, w)
                    nativePoints[i] = new XYZW
                    {
                        wx = v.X * w,
                        wy = v.Y * w,
                        wz = 0.0,
                        w = w,
                    };
                }

                var result = LNLibBezierCurve.GetRationalPointOnCurveByBernstein(
                    Degree,
                    nativePoints,
                    count,
                    t
                );

                // IMPORTANT: Convert Homogeneous Result back to Euclidean
                // P_euc = (wx/w, wy/w)
                // We return X, Y as position and Z as the interpolated weight
                if (System.Math.Abs(result.w) > 1e-9)
                {
                    double invW = 1.0 / result.w;
                    return new Vector3(
                        (float)(result.wx * invW),
                        (float)(result.wy * invW),
                        (float)result.w
                    );
                }

                // Fallback for zero weight (shouldn't happen with valid curves)
                return new Vector3((float)result.wx, (float)result.wy, (float)result.w);
            }
            finally
            {
                pool.Return(nativePoints);
            }
        }

        // Helper to get pure 2D point
        public Vector2 GetPointOnCurve2D(float t)
        {
            var v3 = GetPointOnCurve(t);
            return new Vector2(v3.X, v3.Y);
        }
    }
}
