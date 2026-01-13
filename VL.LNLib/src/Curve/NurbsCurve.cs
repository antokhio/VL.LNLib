using LNLibSharp;

namespace VL.LNLib.Curve
{
    public record struct NurbsCurve<T> : IDisposable
    {
        /// <summary>
        /// Holds the instance of the native LN_NurbsCurve struct.
        /// </summary>
        public LN_NurbsCurve NativeCurve { get; set; }

        /// <summary>
        /// The degree of the curve.
        /// </summary>
        public int Degree { get; set; }

        /// <summary>
        /// Control points converted to Vector2.
        /// Note: This conversion assumes the native control points are weighted (XYZW) and performs the division by W.
        /// </summary>
        public IReadOnlyList<T> ControlPoints { get; set; }

        /// <summary>
        /// The knot vector converted to float.
        /// </summary>
        public IReadOnlyList<float> Knots { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NurbsCurve"/> struct from a native <see cref="LN_NurbsCurve"/>.
        /// </summary>
        /// <param name="nativeCurve">The native curve struct.</param>
        public NurbsCurve(LN_NurbsCurve nativeCurve)
        {
            NativeCurve = nativeCurve;
            Degree = nativeCurve.degree;
            Knots = NurbsCurveHelper.MarshalKnots(nativeCurve.knot_vector, nativeCurve.knot_count);
            ControlPoints = NurbsCurveHelper.MarshalControlPoints<T>(
                nativeCurve.control_points,
                nativeCurve.control_point_count
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NurbsCurve"/> struct, allocating native memory for it.
        /// Auto-generates a clamped uniform knot vector.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <param name="controlPoints">The control points.</param>
        public NurbsCurve(int degree, IReadOnlyList<T> controlPoints)
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            if (degree < 1)
                throw new ArgumentOutOfRangeException(nameof(degree));

            Degree = degree;
            ControlPoints = controlPoints;
            NativeCurve = NurbsCurveHelper.CreateNative(controlPoints, degree, out var knots);
            Knots = knots;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NurbsCurve"/> struct with explicit knots, allocating native memory for it.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <param name="controlPoints">The control points.</param>
        /// <param name="knots">The knot vector.</param>
        public NurbsCurve(int degree, IReadOnlyList<T> controlPoints, IReadOnlyList<float> knots)
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            if (knots == null)
                throw new ArgumentNullException(nameof(knots));
            if (degree < 1)
                throw new ArgumentOutOfRangeException(nameof(degree));

            Degree = degree;
            ControlPoints = controlPoints;
            Knots = knots;
            NativeCurve = NurbsCurveHelper.CreateNative(controlPoints, degree, knots);
        }

        /// <summary>
        /// Evaluates the curve at parameter t.
        /// </summary>
        /// <param name="t">The parameter value.</param>
        /// <returns>The point on the curve.</returns>
        public T GetPointOnCurve(float t)
        {
            var result = LNLibNurbsCurve.GetPointOnCurve(NativeCurve, t);
            return NurbsCurveHelper.FromXYZ<T>(result);
        }

        /// <summary>
        /// Reparametrize the curve within the given domain.
        /// </summary>
        /// <param name="min">Minimum value of the new domain.</param>
        /// <param name="max">Maximum value of the new domain.</param>
        /// <returns>A new NurbsCurve instance representing the reparametrized curve.</returns>
        public NurbsCurve<T> Reparametrize(double min, double max)
        {
            LNLibNurbsCurve.Reparametrize(NativeCurve, min, max, out var result);
            return new NurbsCurve<T>(result);
        }

        /// <summary>
        /// Frees the unmanaged memory allocated for the NativeCurve.
        /// Should only be called if this struct was created via the (degree, points) constructor
        /// or if you own the native memory.
        /// </summary>
        public void Dispose()
        {
            // Note: Since this is a struct, Dispose modifies the copy if not passed by ref,
            // but here we modify the fields of 'this'.
            // Users must be careful not to double-dispose copies.
            var native = NativeCurve;
            NurbsCurveHelper.FreeNative(ref native);
            NativeCurve = native;
        }

        /// <summary>
        /// Implicitly converts an LN_NurbsCurve to a NurbsCurve.
        /// </summary>
        public static implicit operator NurbsCurve<T>(LN_NurbsCurve native) =>
            new NurbsCurve<T>(native);
    }
}
