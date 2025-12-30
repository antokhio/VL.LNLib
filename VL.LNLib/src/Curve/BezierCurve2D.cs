using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    public record BezierCurve2D : BezierCurve<Vector2>
    {
        public BezierCurve2D(int degree, IReadOnlyList<Vector2> controlPoints)
            : base(degree, controlPoints) { }

        public override Vector2 GetPointOnCurve(float t)
        {
            int count = ControlPoints.Count;
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

                var result = LNLibBezierCurve.GetPointOnCurveByBernstein(
                    Degree,
                    nativePoints,
                    count,
                    t
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
