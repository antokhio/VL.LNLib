using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Helpers;

namespace VL.LNLib.Curve
{
    /// <summary>
    /// Standard 2D NURBS Curve (Non-Rational).
    /// Input: Vector2. Internally converted to Homogeneous (Z=0, W=1) for calculations.
    /// </summary>
    public record NurbsCurve2D : NurbsCurve<Vector2>
    {
        public NurbsCurve2D(
            int degree,
            IReadOnlyList<double> knots,
            IReadOnlyList<Vector2> controlPoints
        )
            : base(degree, knots, controlPoints) { }

        public override XYZW[] ToNativeControlPoints()
        {
            var count = ControlPoints.Count;
            var native = new XYZW[count];
            for (int i = 0; i < count; i++)
            {
                var v = ControlPoints[i];
                // Non-rational 2D: Z=0, Weight=1
                native[i] = new XYZW
                {
                    wx = v.X,
                    wy = v.Y,
                    wz = 0.0,
                    w = 1.0,
                };
            }
            return native;
        }

        public override Vector2 GetPointOnCurve(float t)
        {
            Vector2 result = default;
            NurbsCurveInterop.WithNativeCurve(
                this,
                nativeCurve =>
                {
                    LNLibNurbsCurve.Reparametrize(nativeCurve, 0.0, 1.0, out var normalizedCurve);

                    XYZ point = LNLibNurbsCurve.GetPointOnCurve(normalizedCurve, t);
                    result = new Vector2((float)point.x, (float)point.y);
                }
            );
            return result;
        }
    }
}
