using System.Runtime.InteropServices;
using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Extensions
{
    public static class NurbsCurveExtensions
    {
        public static double[] GetKnots(this LN_NurbsCurve nativeCurve)
        {
            var arr = new double[nativeCurve.knot_count];
            Marshal.Copy(nativeCurve.knot_vector, arr, 0, nativeCurve.knot_count);
            return arr;
        }

        public static Vector2[] GetControlPoints(this LN_NurbsCurve nativeCurve)
        {
            var count = nativeCurve.control_point_count;
            var arr = new Vector2[count];
            var ptr = nativeCurve.control_points;
            var stride = Marshal.SizeOf<XYZW>();

            for (int i = 0; i < count; i++)
            {
                var p = Marshal.PtrToStructure<XYZW>(ptr + i * stride);

                // Project Homogeneous (XYZW) to Cartesian (Vector2).
                // NurbsCurve2D uses Vector2 which is non-rational (w=1).
                var w = p.w;

                // Treat close-to-zero weight as 1.0 to avoid division by zero or infinity.
                if (Math.Abs(w) < 1e-9)
                    w = 1.0;

                arr[i] = new Vector2((float)(p.wx / w), (float)(p.wy / w));
            }

            return arr;
        }

        //public static NurbsCurve2D? Reparametrize(
        //    this NurbsCurve2D? curve,
        //    float min = 0f,
        //    float max = 1f
        //)
        //{
        //    if (curve is null || curve.IsValid is false)
        //        return curve!;

        //    LNLibNurbsCurve.Reparametrize(curve.NativeCurve, min, max, out var reparametrizedCurve);

        //    var newCurve = new NurbsCurve2D(reparametrizedCurve);

        //    return newCurve;
        //}
    }
}
