using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Surface.VL.LNLib.Surface;

namespace VL.LNLib.Surface
{
    public record BezierSurface2D : BezierSurface<Vector2>
    {
        public BezierSurface2D(
            int controlPointsResolutionX,
            int controlPointsResolutionY,
            IReadOnlyList<Vector2> controlPoints
        )
            : base(controlPointsResolutionX, controlPointsResolutionY, controlPoints) { }

        public override Vector2 GetPointOnSurface(Vector2 uv)
        {
            int count = ControlPoints.Count;
            int rows = ControlPointsResolutionX;
            int cols = ControlPointsResolutionY;

            var pool = ArrayPool<XYZ>.Shared;
            XYZ[] nativePoints = pool.Rent(count);

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var v = ControlPoints[i];
                    nativePoints[i] = new XYZ
                    {
                        x = v.X,
                        y = v.Y,
                        z = 0.0,
                    };
                }

                var nativeUV = new UV { u = uv.X, v = uv.Y };

                var result = LNLibBezierSurface.GetPointOnSurfaceByDeCasteljau(
                    DegreeU,
                    DegreeV,
                    nativePoints,
                    rows,
                    cols,
                    nativeUV
                );

                return new Vector2((float)result.x, (float)result.y);
            }
            finally
            {
                pool.Return(nativePoints);
            }
        }
    }
}
