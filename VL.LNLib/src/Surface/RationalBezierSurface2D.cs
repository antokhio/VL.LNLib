using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Surface.VL.LNLib.Surface;

namespace VL.LNLib.Surface
{
    public record RationalBezierSurface2D : BezierSurface<Vector3>
    {
        public RationalBezierSurface2D(
            int controlPointsResolutionX,
            int controlPointsResolutionY,
            IReadOnlyList<Vector3> controlPoints
        )
            : base(controlPointsResolutionX, controlPointsResolutionY, controlPoints) { }

        public override Vector3 GetPointOnSurface(Vector2 uv)
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
                    double w = v.Z; // Z component is Weight in 2D Rational

                    // Convert to Homogeneous: (x*w, y*w, 0, w)
                    nativePoints[i] = new XYZW
                    {
                        wx = v.X * w,
                        wy = v.Y * w,
                        wz = 0.0,
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

                // Convert back to Euclidean: (wx/w, wy/w, w)
                if (System.Math.Abs(result.w) > 1e-9)
                {
                    double invW = 1.0 / result.w;
                    return new Vector3(
                        (float)(result.wx * invW),
                        (float)(result.wy * invW),
                        (float)result.w
                    );
                }

                return new Vector3((float)result.wx, (float)result.wy, (float)result.w);
            }
            finally
            {
                pool.Return(nativePoints);
            }
        }

        public Vector2 GetPointOnSurface2D(Vector2 uv)
        {
            var v3 = GetPointOnSurface(uv);
            return new Vector2(v3.X, v3.Y);
        }
    }
}
