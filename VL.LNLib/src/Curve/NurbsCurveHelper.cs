using System.Runtime.InteropServices;

namespace VL.LNLib.Curve
{
    internal static class NurbsCurveHelper
    {
        internal static float[] MarshalKnots(IntPtr ptr, int count)
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
    }
}
