//using LNLibSharp;
//using Stride.Core.Mathematics;
//using VL.LNLib.Base;

//namespace VL.LNLib.Curve
//{
//    /// <summary>
//    /// Abstract base record for high-level NURBS curve definitions.
//    /// </summary>
//    public abstract partial record NurbsCurve<T> : Validatable
//    {
//        /// <summary>
//        /// The degree of the curve (p).
//        /// 1 = Linear (Polyline), 2 = Quadratic (Arcs), 3 = Cubic (Standard smooth curve).
//        /// </summary>
//        public int Degree { get; init; }

//        /// <summary>
//        /// The list of control points.
//        /// </summary>
//        public IReadOnlyList<T> ControlPoints { get; init; }

//        /// <summary>
//        /// Count of control points.
//        /// </summary>
//        public int ControlPointsCount => ControlPoints.Count;

//        /// <summary>
//        /// The knot vector.
//        /// Count must equal ControlPoints.Count + Degree + 1.
//        /// </summary>
//        public IReadOnlyList<double> Knots { get; init; }

//        /// <summary>
//        /// Count of knots.
//        /// </summary>
//        public int KnotsCount => Knots.Count;

//        protected NurbsCurve(
//            int degree,
//            IReadOnlyList<double> knots,
//            IReadOnlyList<T> controlPoints
//        )
//        {
//            Degree = degree;
//            Knots = knots ?? throw new ArgumentNullException(nameof(knots));
//            ControlPoints = controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
//        }

//        /// <summary>
//        /// Copy constructor for record cloning (with-expressions).
//        /// We manually copy properties and leave native handles default (null/zero)
//        /// to ensure the new instance rebuilds its native representation lazily.
//        /// </summary>
//        protected NurbsCurve(NurbsCurve<T> original)
//            : base(original)
//        {
//            Degree = original.Degree;
//            Knots = original.Knots;
//            ControlPoints = original.ControlPoints;
//        }

//        public override void Validate()
//        {
//            VALIDATE_ARGUMENT(Degree > 0, nameof(Degree), "Degree must be greater than zero.");
//            VALIDATE_ARGUMENT(
//                KnotsCount > 0,
//                nameof(Knots),
//                "Knots size must be greater than zero"
//            );
//            VALIDATE_ARGUMENT(
//                LNLibValidationUtils.IsValidKnotVector(Knots.ToArray(), KnotsCount) == 1,
//                "knotVector",
//                "KnotVector must be a nondecreasing sequence of real numbers."
//            );
//            VALIDATE_ARGUMENT(
//                ControlPointsCount > 0,
//                "controlPoints",
//                "ControlPoints must contain one point at least."
//            );
//            VALIDATE_ARGUMENT(
//                LNLibValidationUtils.IsValidNurbs(Degree, KnotsCount, ControlPointsCount) == 1,
//                "controlPoints",
//                "Arguments must be fit: m = n + p + 1"
//            );
//        }

//        public void Reparametrize(float min = 0f, float max = 1f)
//        {
//            if (this is null || IsValid is false)
//                return;

//            LNLibNurbsCurve.Reparametrize(NativeCurve, min, max, out var reparametrizedCurve);

//            _nativeCurve = reparametrizedCurve;
//        }

//        public void GetPointOnCurve(float t, out Vector2 point)
//        {
//            point = default;

//            if (this is null || IsValid is false)
//                return;

//            XYZ nativePoint = LNLibNurbsCurve.GetPointOnCurve(NativeCurve, t);
//            point = new Vector2((float)nativePoint.x, (float)nativePoint.y);
//        }

//        public void GetPointOnCurve(float t, out Vector3 point)
//        {
//            point = default;
//            if (this is null || IsValid is false)
//                return;

//            XYZ nativePoint = LNLibNurbsCurve.GetPointOnCurve(NativeCurve, t);
//            point = new Vector3((float)nativePoint.x, (float)nativePoint.y, (float)nativePoint.z);
//        }
//    }
//}
