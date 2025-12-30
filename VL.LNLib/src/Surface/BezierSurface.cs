using Stride.Core.Mathematics;

namespace VL.LNLib.Surface
{
    namespace VL.LNLib.Surface
    {
        /// <summary>
        /// Abstract base class for Bezier Surface wrappers.
        /// </summary>
        /// <typeparam name="T">Control point type (e.g. Vector3, Vector4).</typeparam>
        public abstract record BezierSurface<T>
        {
            public int ControlPointsResolutionX { get; set; }
            public int ControlPointsResolutionY { get; set; }

            public int DegreeU => ControlPointsResolutionX - 1;
            public int DegreeV => ControlPointsResolutionY - 1;

            /// <summary>
            /// Flattened list of control points in row-major order (U then V).
            /// Count must be (DegreeU + 1) * (DegreeV + 1).
            /// </summary>
            public IReadOnlyList<T> ControlPoints { get; set; }

            public bool IsValid
            {
                get
                {
                    if (DegreeU <= 0 || DegreeV <= 0)
                        return false;
                    if (ControlPoints == null || ControlPoints.Count == 0)
                        return false;

                    int requiredCount = (DegreeU + 1) * (DegreeV + 1);
                    return ControlPoints.Count == requiredCount;
                }
            }

            protected BezierSurface(
                int controlPointsResolutionX,
                int controlPointsResolutionY,
                IReadOnlyList<T> controlPoints
            )
            {
                ControlPointsResolutionX = controlPointsResolutionX;
                ControlPointsResolutionY = controlPointsResolutionY;
                ControlPoints =
                    controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
            }

            /// <summary>
            /// Computes a point on the surface at the given UV coordinate.
            /// </summary>
            /// <param name="uv">Coordinate (X=u, Y=v), usually in [0,1] range.</param>
            public abstract T GetPointOnSurface(Vector2 uv);
        }
    }
}
