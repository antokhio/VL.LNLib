using System.Buffers;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    public record BezierCurve3D : BezierCurve<Vector3>
    {
        public BezierCurve3D(int controlPointsResolution, IReadOnlyList<Vector3> controlPoints)
            : base(controlPointsResolution, controlPoints) { }

        public override Vector3 GetPointOnCurve(float t)
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
                        z = v.Z,
                    };
                }

                var result = LNLibBezierCurve.GetPointOnCurveByBernstein(
                    Degree,
                    nativePoints,
                    count,
                    t
                );
                return new Vector3((float)result.x, (float)result.y, (float)result.z);
            }
            finally
            {
                pool.Return(nativePoints);
            }
        }
    }
}
