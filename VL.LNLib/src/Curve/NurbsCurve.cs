using System.Runtime.CompilerServices;
using LNLibSharp;
using Stride.Core.Mathematics;

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
        /// The approximate length of the curve. Calculated upon creation.
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        /// Control points converted to Vector2 or Vector3.
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
            Length = (float)
                LNLibNurbsCurve.ApproximateLength(
                    NativeCurve,
                    IntegratorType.INTEGRATOR_GAUSS_LEGENDRE
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
            Length = (float)
                LNLibNurbsCurve.ApproximateLength(
                    NativeCurve,
                    IntegratorType.INTEGRATOR_GAUSS_LEGENDRE
                );
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
            Length = (float)
                LNLibNurbsCurve.ApproximateLength(
                    NativeCurve,
                    IntegratorType.INTEGRATOR_GAUSS_LEGENDRE
                );
        }

        /// <summary>
        /// Evaluates the curve at parameter t.
        /// </summary>
        /// <param name="t">The parameter value.</param>
        /// <returns>The point on the curve.</returns>
        public T GetPointOnCurve(float t)
        {
            ThrowIfNotAssigned();
            var result = LNLibNurbsCurve.GetPointOnCurve(NativeCurve, t);
            return NurbsCurveHelper.FromXYZ<T>(result);
        }

        /// <summary>
        /// Evaluates the curve at a normalized position along its length (0 to 1).
        /// This method compensates for non-uniform parameterization.
        /// </summary>
        /// <param name="factor">The normalized length factor (0.0 to 1.0).</param>
        /// <returns>The point on the curve.</returns>
        public T GetPointAt(float factor)
        {
            ThrowIfNotAssigned();
            // Calculate target length from factor
            float targetLength = factor * Length;

            // Get parameter t corresponding to that length
            var t = GetParamByLength(targetLength);

            // Get point at parameter t
            var result = LNLibNurbsCurve.GetPointOnCurve(NativeCurve, t);
            return NurbsCurveHelper.FromXYZ<T>(result);
        }

        /// <summary>
        /// Calculates the normal vector on the curve at parameter t.
        /// </summary>
        /// <param name="t">The parameter value.</param>
        /// <param name="upVector">Optional up-vector for stability. Defaults to Y-Up.</param>
        /// <returns>The normal vector.</returns>
        public T GetNormal(float t, T? upVector = default)
        {
            ThrowIfNotAssigned();

            // Get normalized tangent
            var tangentT = GetTangent(t);

            if (typeof(T) == typeof(Vector3))
            {
                var tangent = Unsafe.As<T, Vector3>(ref tangentT);
                var up = upVector == null ? Vector3.UnitY : Unsafe.As<T, Vector3>(ref upVector);

                var binormal = Vector3.Cross(tangent, up);

                // If Tangent is parallel to Up, pick a fallback
                if (binormal.LengthSquared() < 1e-5f)
                {
                    // Fallback: try UnitX, if still parallel, try UnitZ
                    binormal = Vector3.Cross(tangent, Vector3.UnitX);
                    if (binormal.LengthSquared() < 1e-5f)
                        binormal = Vector3.Cross(tangent, Vector3.UnitZ);
                }

                binormal = Vector3.Normalize(binormal);

                // Normal = Cross(Binormal, Tangent)
                var normal = Vector3.Cross(binormal, tangent);

                return Unsafe.As<Vector3, T>(ref normal);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                var tangent = Unsafe.As<T, Vector2>(ref tangentT);
                if (tangent.LengthSquared() > 1e-6f)
                    tangent = Vector2.Normalize(tangent);

                // 2D Normal: (-y, x)
                Vector2 normal = new Vector2(-tangent.Y, tangent.X);
                return Unsafe.As<Vector2, T>(ref normal);
            }

            throw new NotSupportedException($"Type {typeof(T)} not supported.");
        }

        /// <summary>
        /// Calculates the normal vector on the curve at a normalized position along its length (0 to 1).
        /// </summary>
        /// <param name="factor">The normalized length factor (0.0 to 1.0).</param>
        /// <param name="upVector">Optional up-vector for 3D stability.</param>
        /// <returns>The normal vector.</returns>
        public T GetNormalAt(float factor, T upVector)
        {
            ThrowIfNotAssigned();
            // Calculate target length from factor
            float targetLength = factor * Length;

            // Get parameter t corresponding to that length
            var t = GetParamByLength(targetLength);

            return GetNormal(t, upVector);
        }

        /// <summary>
        /// Calculates the tangent vector on the curve at parameter t.
        /// </summary>
        /// <param name="t">The parameter value.</param>
        /// <returns>The normalized tangent vector.</returns>
        public T GetTangent(float t)
        {
            ThrowIfNotAssigned();

            // Calculate derivatives (Order 1 gives Point [0] and 1st Derivative [1])
            var derivatives = new XYZ[2];
            LNLibNurbsCurve.ComputeRationalCurveDerivatives(NativeCurve, 1, t, derivatives);

            // derivatives[1] is the tangent vector (unnormalized)
            var vec = NurbsCurveHelper.FromXYZ<T>(derivatives[1]);
            var normalizedVec = NurbsCurveHelper.Normalize(vec);

            return normalizedVec;
        }

        /// <summary>
        /// Calculates the parameter value on the curve that corresponds to the specified arc length from the start of
        /// the curve.
        /// </summary>
        /// <param name="length">The arc length, in curve units, from the start of the curve for which to find the corresponding parameter
        /// value. Must be non-negative and less than or equal to the total length of the curve.</param>
        /// <returns>The parameter value on the curve that is located at the specified arc length from the start of the curve.</returns>
        public float GetParamByLength(float length)
        {
            ThrowIfNotAssigned();

            return (float)
                LNLibNurbsCurve.GetParamByLength(
                    NativeCurve,
                    length,
                    IntegratorType.INTEGRATOR_GAUSS_LEGENDRE
                );
        }

        /// <summary>
        /// Manually calculates the approximate length of the curve.
        /// Usually not needed as Length is cached on creation.
        /// </summary>
        public float ApproximateLength(
            IntegratorType integrator = IntegratorType.INTEGRATOR_GAUSS_LEGENDRE
        )
        {
            ThrowIfNotAssigned();
            return (float)LNLibNurbsCurve.ApproximateLength(NativeCurve, integrator);
        }

        /// <summary>
        /// Reparametrize the curve within the given domain.
        /// </summary>
        /// <param name="min">Minimum value of the new domain.</param>
        /// <param name="max">Maximum value of the new domain.</param>
        /// <returns>A new NurbsCurve instance representing the reparametrized curve.</returns>
        public NurbsCurve<T> Reparametrize(float min, float max)
        {
            ThrowIfNotAssigned();
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
        /// Checks if the native curve is initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if native curve pointers are zero.</exception>
        private void ThrowIfNotAssigned()
        {
            if (NativeCurve.control_points == IntPtr.Zero || NativeCurve.knot_vector == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    "NurbsCurve is not initialized. Ensure it has been created with valid control points and degree."
                );
            }
        }

        /// <summary>
        /// Implicitly converts an LN_NurbsCurve to a NurbsCurve.
        /// </summary>
        public static implicit operator NurbsCurve<T>(LN_NurbsCurve native) =>
            new NurbsCurve<T>(native);
    }
}
