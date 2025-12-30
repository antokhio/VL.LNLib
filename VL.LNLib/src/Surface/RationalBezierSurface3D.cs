using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Surface.VL.LNLib.Surface;

namespace VL.LNLib.Surface
{
    /// <summary>
    /// Rational 3D Bezier Surface using Vector4 control points (X, Y, Z, Weight).
    /// </summary>
    public record RationalBezierSurface3D : BezierSurface<Vector4>
    {
        public RationalBezierSurface3D(
            int controlPointsResolutionX,
            int controlPointsResolutionY,
            IReadOnlyList<Vector4> controlPoints
        )
            : base(controlPointsResolutionX, controlPointsResolutionY, controlPoints) { }

        public override Vector4 GetPointOnSurface(Vector2 uv)
        {
            int count = ControlPoints.Count;
            int rows = ControlPointsResolutionX;
            int cols = ControlPointsResolutionY;

            var pool = ArrayPool<XYZW>.Shared;
            XYZW[] nativePoints = pool.Rent(count);

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var v = ControlPoints[i];
                    double w = v.W;

                    // Convert to Homogeneous Coordinates: (x*w, y*w, z*w, w)
                    nativePoints[i] = new XYZW
                    {
                        wx = v.X * w,
                        wy = v.Y * w,
                        wz = v.Z * w,
                        w = w,
                    };
                }

                var nativeUV = new UV { u = uv.X, v = uv.Y };

                var result = LNLibBezierSurface.GetRationalPointOnSurfaceByDeCasteljau(
                    DegreeU,
                    DegreeV,
                    nativePoints,
                    rows,
                    cols,
                    nativeUV
                );

                // Convert back to Euclidean: (wx/w, wy/w, wz/w, w)
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

        /// <summary>
        /// Helper to get the 3D position (Euclidean) from the surface.
        /// </summary>
        public Vector3 GetPointOnSurface3D(Vector2 uv)
        {
            var v4 = GetPointOnSurface(uv);
            return new Vector3(v4.X, v4.Y, v4.Z);
        }
    }
}
