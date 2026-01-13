//namespace VL.LNLib.Curve
//{
//    /// <summary>
//    /// Rational 3D NURBS Curve.
//    /// </summary>
//    public record RationalNurbsCurve3D : NurbsCurve<Vector4>
//    {
//        public RationalNurbsCurve3D(
//            int degree,
//            IReadOnlyList<double> knots,
//            IReadOnlyList<Vector4> controlPoints
//        )
//            : base(degree, knots, controlPoints) { }

//        public override XYZW[] ToNativeControlPoints()
//        {
//            var count = ControlPoints.Count;
//            var native = new XYZW[count];
//            for (int i = 0; i < count; i++)
//            {
//                // Use Extension
//                native[i] = ControlPoints[i].ToHomogeneousXYZW();
//            }
//            return native;
//        }

//        public override Vector4 GetPointOnCurve(float t)
//        {
//            Vector4 result = default;
//            NurbsCurveInterop.WithNativeCurve(
//                this,
//                nativeCurve =>
//                {
//                    XYZ point = LNLibNurbsCurve.GetPointOnCurve(nativeCurve, t);
//                    result = new Vector4((float)point.x, (float)point.y, (float)point.z, 0.0f);
//                }
//            );
//            return result;
//        }
//    }
//}
