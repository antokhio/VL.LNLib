using System.Runtime.InteropServices;
using Stride.Core.Mathematics;

namespace VL.LNLib.Curve._2D
{
    internal static class NurbsCurve2DHelper
    {
        public static Vector2[] MarshalControlPointsToVector(IntPtr ptr, int count)
        {
            if (ptr == IntPtr.Zero || count == 0)
                return Array.Empty<Vector2>();

            int dimension = 4;
            int totalCount = count * dimension;

            // Read all doubles from the pointer
            var rawPoints = new double[totalCount];
            Marshal.Copy(ptr, rawPoints, 0, totalCount);

            var result = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                int index = i * dimension;
                double x = rawPoints[index];
                double y = rawPoints[index + 1];
                // z is present in data but unused for Vector2
                double w = rawPoints[index + 3];

                float scale = w != 0.0 ? (float)(1.0 / w) : 1.0f;

                result[i] = new Vector2((float)x * scale, (float)y * scale);
            }

            return result;
        }
    }
}
