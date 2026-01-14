using LNLibSharp;

namespace VL.LNLib.Curve
{
    /// <summary>
    /// Abstract base record for NURBS curves.
    /// Handles native resource management and common curve operations.
    /// </summary>
    /// <typeparam name="T">Vector2 or Vector3</typeparam>
    public abstract record NurbsCurve<T>
        where T : struct
    {
        /// <summary>
        /// Holds the instance of the native LN_NurbsCurve struct.
        /// </summary>
        public LN_NurbsCurve NativeCurve { get; set; }

        /// <summary>
        /// The degree of the curve.
        /// </summary>
        public int Degree { get; init; }

        /// <summary>
        /// The approximate length of the curve. Calculated upon creation.
        /// </summary>
        public float Length { get; init; }

        /// <summary>
        /// Control points Vector2 or Vector3.
        /// </summary>
        public IReadOnlyList<T> ControlPoints { get; init; }

        /// <summary>
        /// The knot vector converted to float.
        /// </summary>
        public IReadOnlyList<float> Knots { get; init; }
    }
}
