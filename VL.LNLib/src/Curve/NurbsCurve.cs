using LNLibSharp;

namespace VL.LNLib.Curve
{
    /// <summary>
    /// Abstract base record for high-level NURBS curve definitions.
    /// </summary>
    public abstract record NurbsCurve<T>
    {
        /// <summary>
        /// The degree of the curve (p).
        /// 1 = Linear (Polyline), 2 = Quadratic (Arcs), 3 = Cubic (Standard smooth curve).
        /// </summary>
        public int Degree { get; init; }

        /// <summary>
        /// The list of control points.
        /// </summary>
        public IReadOnlyList<T> ControlPoints { get; init; }

        /// <summary>
        /// Count of control points.
        /// </summary>
        public int ControlPointCount => ControlPoints.Count;

        /// <summary>
        /// The knot vector.
        /// Count must equal ControlPoints.Count + Degree + 1.
        /// </summary>
        public IReadOnlyList<double> Knots { get; init; }

        public bool IsValid
        {
            get
            {
                if (Degree < 1)
                    return false;
                if (Knots == null || ControlPoints == null)
                    return false;
                if (Knots.Count == 0 || ControlPoints.Count == 0)
                    return false;

                // Strict NURBS Definition Requirement
                return Knots.Count == ControlPoints.Count + Degree + 1;
            }
        }

        protected NurbsCurve(
            int degree,
            IReadOnlyList<double> knots,
            IReadOnlyList<T> controlPoints
        )
        {
            Degree = degree;
            Knots = knots ?? throw new ArgumentNullException(nameof(knots));
            ControlPoints = controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
        }

        public abstract XYZW[] ToNativeControlPoints();
    }
}
