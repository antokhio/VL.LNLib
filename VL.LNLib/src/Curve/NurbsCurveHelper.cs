using System.Runtime.InteropServices;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve
{
    internal class NurbsCurveHelper
    {
        public static float[] MarshalKnots(IntPtr ptr, int count)
        {
            if (ptr == IntPtr.Zero || count == 0)
                return Array.Empty<float>();

            var doubleKnots = new double[count];
            Marshal.Copy(ptr, doubleKnots, 0, count);

            var floatKnots = new float[count];
            for (int i = 0; i < count; i++)
                floatKnots[i] = (float)doubleKnots[i];

            return floatKnots;
        }

        public static T[] MarshalControlPoints<T>(IntPtr ptr, int count)
        {
            if (ptr == IntPtr.Zero || count == 0)
                return Array.Empty<T>();

            int dimension = 4;
            int totalCount = count * dimension;

            var rawPoints = new double[totalCount];
            Marshal.Copy(ptr, rawPoints, 0, totalCount);

            var result = new T[count];
            var type = typeof(T);
            bool isVector2 = type == typeof(Vector2);
            bool isVector3 = type == typeof(Vector3);

            if (!isVector2 && !isVector3)
                throw new NotSupportedException(
                    $"Type {type} is not supported. Only Vector2 and Vector3."
                );

            for (int i = 0; i < count; i++)
            {
                int index = i * dimension;
                double x = rawPoints[index];
                double y = rawPoints[index + 1];
                double z = rawPoints[index + 2];
                double w = rawPoints[index + 3];

                float scale = w != 0.0 ? (float)(1.0 / w) : 1.0f;

                if (isVector2)
                    result[i] = (T)(object)new Vector2((float)x * scale, (float)y * scale);
                else
                    result[i] = (T)
                        (object)new Vector3((float)x * scale, (float)y * scale, (float)z * scale);
            }

            return result;
        }

        public static LN_NurbsCurve CreateNative<T>(
            IReadOnlyList<T> controlPoints,
            int degree,
            out IReadOnlyList<float> knots
        )
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));

            // 1. Generate Knots
            var floatKnots = GenerateClampedKnots(degree, controlPoints.Count);
            knots = floatKnots;

            // 2. Marshal Knots
            IntPtr knotPtr = MarshalKnotsToNative(floatKnots);

            // 3. Marshal Control Points
            IntPtr cpPtr = MarshalControlPointsToNative(controlPoints);

            return new LN_NurbsCurve
            {
                degree = degree,
                knot_count = floatKnots.Length,
                knot_vector = knotPtr,
                control_point_count = controlPoints.Count,
                control_points = cpPtr,
            };
        }

        public static LN_NurbsCurve CreateNative<T>(
            IReadOnlyList<T> controlPoints,
            int degree,
            IReadOnlyList<float> knots
        )
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            if (knots == null)
                throw new ArgumentNullException(nameof(knots));

            // 1. Marshal Knots
            IntPtr knotPtr = MarshalKnotsToNative(knots);

            // 2. Marshal Control Points
            IntPtr cpPtr = MarshalControlPointsToNative(controlPoints);

            return new LN_NurbsCurve
            {
                degree = degree,
                knot_count = knots.Count,
                knot_vector = knotPtr,
                control_point_count = controlPoints.Count,
                control_points = cpPtr,
            };
        }

        private static IntPtr MarshalKnotsToNative(IReadOnlyList<float> knots)
        {
            var doubleKnots = new double[knots.Count];
            for (int i = 0; i < knots.Count; i++)
                doubleKnots[i] = knots[i];

            int knotSize = doubleKnots.Length * sizeof(double);
            IntPtr knotPtr = Marshal.AllocHGlobal(knotSize);
            Marshal.Copy(doubleKnots, 0, knotPtr, doubleKnots.Length);
            return knotPtr;
        }

        private static IntPtr MarshalControlPointsToNative<T>(IReadOnlyList<T> controlPoints)
        {
            int count = controlPoints.Count;
            int dim = 4;
            var doublePoints = new double[count * dim];
            bool isVector2 = typeof(T) == typeof(Vector2);

            for (int i = 0; i < count; i++)
            {
                int baseIdx = i * dim;
                if (isVector2)
                {
                    Vector2 v = (Vector2)(object)controlPoints[i];
                    doublePoints[baseIdx] = v.X;
                    doublePoints[baseIdx + 1] = v.Y;
                    doublePoints[baseIdx + 2] = 0.0;
                    doublePoints[baseIdx + 3] = 1.0;
                }
                else
                {
                    Vector3 v = (Vector3)(object)controlPoints[i];
                    doublePoints[baseIdx] = v.X;
                    doublePoints[baseIdx + 1] = v.Y;
                    doublePoints[baseIdx + 2] = v.Z;
                    doublePoints[baseIdx + 3] = 1.0;
                }
            }

            int cpSize = doublePoints.Length * sizeof(double);
            IntPtr cpPtr = Marshal.AllocHGlobal(cpSize);
            Marshal.Copy(doublePoints, 0, cpPtr, doublePoints.Length);
            return cpPtr;
        }

        public static void FreeNative(ref LN_NurbsCurve curve)
        {
            if (curve.knot_vector != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(curve.knot_vector);
                curve.knot_vector = IntPtr.Zero;
            }
            if (curve.control_points != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(curve.control_points);
                curve.control_points = IntPtr.Zero;
            }
            curve.knot_count = 0;
            curve.control_point_count = 0;
            curve.degree = 0;
        }

        public static T FromXYZ<T>(XYZ point)
        {
            if (typeof(T) == typeof(Vector2))
                return (T)(object)new Vector2((float)point.x, (float)point.y);
            if (typeof(T) == typeof(Vector3))
                return (T)(object)new Vector3((float)point.x, (float)point.y, (float)point.z);

            throw new NotSupportedException($"Type {typeof(T)} not supported.");
        }

        public static float[] GenerateClampedKnots(int degree, int cpCount)
        {
            int knotCount = cpCount + degree + 1;
            var knots = new float[knotCount];

            int internalKnots = knotCount - 2 * (degree + 1);

            for (int i = 0; i <= degree; i++)
                knots[i] = 0f;

            if (internalKnots > 0)
            {
                float step = 1.0f / (internalKnots + 1);
                for (int i = 0; i < internalKnots; i++)
                    knots[degree + 1 + i] = (i + 1) * step;
            }

            for (int i = knotCount - (degree + 1); i < knotCount; i++)
                knots[i] = 1f;

            return knots;
        }
    }
}
