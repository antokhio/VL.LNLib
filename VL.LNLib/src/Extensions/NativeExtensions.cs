using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Extensions
{
    public static class NativeExtensions
    {
        public static double[] ToDoubleArray(this IReadOnlyList<float> values)
        {
            var count = values.Count;
            var arr = new double[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = values[i];
            }
            return arr;
        }

        public static T FromXYZW<T>(ref this XYZW point)
        {
            // Unweight the homogeneous coordinates (wx, wy, wz) by w
            // to get back to Euclidean coordinates for the user types.

            if (typeof(T) == typeof(Vector2))
            {
                return (T)
                    (object)new Vector2((float)(point.wx / point.w), (float)(point.wy / point.w));
            }
            if (typeof(T) == typeof(Vector3))
            {
                return (T)
                    (object)
                        new Vector3(
                            (float)(point.wx / point.w),
                            (float)(point.wy / point.w),
                            (float)(point.wz / point.w)
                        );
            }
            if (typeof(T) == typeof(Vector4))
            {
                // For Vector4, we return (x, y, z, w)
                var w = (float)point.w;
                var invW = w != 0 ? 1.0f / w : 1.0f;
                return (T)
                    (object)
                        new Vector4(
                            (float)(point.wx * invW),
                            (float)(point.wy * invW),
                            (float)(point.wz * invW),
                            w
                        );
            }
            throw new NotSupportedException($"Control point type {typeof(T)} is not supported.");
        }

        public static XYZW[] ToXYZWArray<T>(this IReadOnlyList<T> values)
            where T : struct
        {
            var count = values.Count;
            var arr = new XYZW[count];

            for (int i = 0; i < count; i++)
            {
                var v = values[i];
                arr[i] = v switch
                {
                    Vector2 vec2 => new XYZW
                    {
                        wx = vec2.X,
                        wy = vec2.Y,
                        wz = 0.0,
                        w = 1.0,
                    },
                    Vector3 vec3 => new XYZW
                    {
                        wx = vec3.X,
                        wy = vec3.Y,
                        wz = vec3.Z,
                        w = 1.0,
                    },
                    Vector4 vec4 => new XYZW
                    {
                        wx = vec4.X * vec4.W,
                        wy = vec4.Y * vec4.W,
                        wz = vec4.Z * vec4.W,
                        w = vec4.W,
                    },
                    _ => throw new NotSupportedException(
                        $"Control point type {typeof(T)} is not supported."
                    ),
                };
            }

            return arr;
        }

        public static T ToVector<T>(ref this XYZ point) =>
            typeof(T) switch
            {
                Type t when t == typeof(Vector2) => (T)
                    (object)new Vector2((float)point.x, (float)point.y),
                Type t when t == typeof(Vector3) => (T)
                    (object)new Vector3((float)point.x, (float)point.y, (float)point.z),
                _ => throw new NotSupportedException(
                    $"Conversion to vector type {typeof(T)} is not supported."
                ),
            };

        public static Vector2 ToVector2(ref this XYZ point) =>
            new Vector2((float)point.x, (float)point.y);

        public static Vector3 ToVector3(ref this XYZ point) =>
            new Vector3((float)point.x, (float)point.y, (float)point.z);
    }
}
