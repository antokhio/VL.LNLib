using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Extensions;

namespace VL.LNLib.Curve
{
    /// <summary>
    /// Rational 3D NURBS Curve.
    /// </summary>
    public record RationalNurbsCurve3D : NurbsCurve<Vector4>
    {
        public RationalNurbsCurve3D(
            int degree,
            IReadOnlyList<double> knots,
            IReadOnlyList<Vector4> controlPoints
        )
            : base(degree, knots, controlPoints) { }

        public override XYZW[] ToNativeControlPoints()
        {
            var count = ControlPoints.Count;
            var native = new XYZW[count];
            for (int i = 0; i < count; i++)
            {
                // Use Extension
                native[i] = ControlPoints[i].ToHomogeneousXYZW();
            }
            return native;
        }
    }
}
