//using Stride.Core.Mathematics;

//namespace VL.LNLib.Curve
//{
//    /// <summary>
//    /// Standard 2D NURBS Curve (Non-Rational).
//    /// Input: Vector2. Internally converted to Homogeneous (Z=0, W=1) for calculations.
//    /// </summary>
//    public record NurbsCurve2D : NurbsCurve<Vector2>
//    {
//        public NurbsCurve2D(
//            int degree,
//            IReadOnlyList<double> knots,
//            IReadOnlyList<Vector2> controlPoints
//        )
//            : base(degree, knots, controlPoints) { }
//    }
//}
