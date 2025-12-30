using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    public record RationalBezierCurve3D : BezierCurve<Vector4>
    {
        public RationalBezierCurve3D(int degree, IReadOnlyList<Vector4> controlPoints)
            : base(degree, controlPoints) { }

        public override Vector4 GetPointOnCurve(float t)
        {
            // Input ControlPoints are: X, Y, Z, Weight

            int count = ControlPoints.Count;
            var pool = ArrayPool<XYZW>.Shared;
            XYZW[] nativePoints = pool.Rent(count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var v = ControlPoints[i];
                    double w = v.W;

                    // Convert to Homogeneous
                    nativePoints[i] = new XYZW
                    {
                        wx = v.X * w,
                        wy = v.Y * w,
                        wz = v.Z * w,
                        w = w,
                    };
                }

                var result = LNLibBezierCurve.GetRationalPointOnCurveByBernstein(
                    Degree,
                    nativePoints,
                    count,
                    t
                );

                // Convert back to Euclidean
                if (System.Math.Abs(result.w) > 1e-9)
                {
                    double invW = 1.0 / result.w;
                    return new Vector4(
                        (float)(result.wx * invW),
                        (float)(result.wy * invW),
                        (float)(result.wz * invW),
                        (float)result.w
                    );
                }

                return new Vector4(
                    (float)result.wx,
                    (float)result.wy,
                    (float)result.wz,
                    (float)result.w
                );
            }
            finally
            {
                pool.Return(nativePoints);
            }
        }

        // Helper to get pure 3D point
        public Vector3 GetPointOnCurve3D(float t)
        {
            var v3 = GetPointOnCurve(t);
            return new Vector3(v3.X, v3.Y, v3.Z);
        }
    }
}
