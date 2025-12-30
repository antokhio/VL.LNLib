using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Extensions;

namespace VL.LNLib.Curve
{
    /// <summary>
    /// Standard 3D NURBS Curve (Non-Rational).
    /// </summary>
    public record NurbsCurve3D : NurbsCurve<Vector3>
    {
        public NurbsCurve3D(
            int degree,
            IReadOnlyList<double> knots,
            IReadOnlyList<Vector3> controlPoints
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
