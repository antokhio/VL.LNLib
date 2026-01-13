using System.Runtime.InteropServices;
using LNLibSharp;
using VL.LNLib.Extensions;

namespace VL.LNLib.Nurbs
{
    public static class NurbsCurveHelper
    {
        public static NurbsCurve<T> Reparametrize<T>(
            ref NurbsCurve<T> input,
            float min = 0f,
            float max = 1f
        )
            where T : struct
        {
            if (input is null)
            {
                return null;
            }

            if (input.IsValid is false)
            {
                return null;
            }

            if (input.Curve is null)
            {
                return null;
            }

            LNLibNurbsCurve.Reparametrize(input.Curve.Value, min, max, out var reparametrizedCurve);

            return new NurbsCurve<T>(reparametrizedCurve);
        }

        public static T GetPointOnCurve<T>(ref NurbsCurve<T> input, float t)
            where T : struct
        {
            if (input is null)
            {
                return default;
            }

            if (input.IsValid is false)
            {
                return default;
            }

            if (input.Curve is null)
            {
                return default;
            }

            var point = LNLibNurbsCurve.GetPointOnCurve(input.Curve.Value, (double)t);

            return point.ToVector<T>();
        }

        public static LN_NurbsCurve Build<T>(
            IReadOnlyList<T> controlPoints,
            IReadOnlyList<float> knots,
            int degree,
            ref GCHandle knotsHandle,
            ref GCHandle cpsHandle
        )
            where T : struct
        {
            if (knotsHandle.IsAllocated)
                knotsHandle.Free();

            if (cpsHandle.IsAllocated)
                cpsHandle.Free();

            var knotsArray = knots.ToDoubleArray();
            knotsHandle = GCHandle.Alloc(knotsArray, GCHandleType.Pinned);

            var cpsArray = controlPoints.ToXYZWArray();
            cpsHandle = GCHandle.Alloc(cpsArray, GCHandleType.Pinned);

            try
            {
                return new LN_NurbsCurve
                {
                    degree = degree,
                    knot_vector = knotsHandle.AddrOfPinnedObject(),
                    knot_count = knotsArray.Length,
                    control_points = cpsHandle.AddrOfPinnedObject(),
                    control_point_count = cpsArray.Length,
                };
            }
            catch
            {
                if (knotsHandle.IsAllocated)
                    knotsHandle.Free();
                if (cpsHandle.IsAllocated)
                    cpsHandle.Free();

                throw new ArgumentOutOfRangeException(
                    "Failed to build LN_NurbsCurve from NurbsCurve"
                );
            }
        }
    }
}
