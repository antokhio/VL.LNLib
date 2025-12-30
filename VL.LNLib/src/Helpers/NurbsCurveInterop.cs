using System.Runtime.InteropServices;
using LNLibSharp;
using Stride.Core.Mathematics;
using VL.LNLib.Curve;

namespace VL.LNLib.Helpers
{
    internal static class NurbsCurveInterop
    {
        /// <summary>
        /// Executes an action with a Native LN_NurbsCurve struct prepared from a Managed NurbsCurve.
        /// Handles array pinning and temporary structure creation.
        /// </summary>
        public static void WithNativeCurve<T>(
            NurbsCurve<T> managedCurve,
            Action<LN_NurbsCurve> action
        )
        {
            if (managedCurve == null || !managedCurve.IsValid)
                throw new ArgumentException("Invalid NurbsCurve");

            // 1. Prepare Arrays
            var knotsArray = managedCurve.Knots.ToArray(); // Need array for pinning
            var cpArray = managedCurve.ToNativeControlPoints();

            // 2. Pin Memory
            GCHandle knotsHandle = GCHandle.Alloc(knotsArray, GCHandleType.Pinned);
            GCHandle cpHandle = GCHandle.Alloc(cpArray, GCHandleType.Pinned);

            try
            {
                // 3. Create Native Struct
                var nativeCurve = new LN_NurbsCurve
                {
                    degree = managedCurve.Degree,
                    knot_vector = knotsHandle.AddrOfPinnedObject(),
                    knot_count = knotsArray.Length,
                    control_points = cpHandle.AddrOfPinnedObject(),
                    control_point_count = cpArray.Length,
                };

                // 4. Execute
                action(nativeCurve);
            }
            finally
            {
                // 5. Cleanup
                knotsHandle.Free();
                cpHandle.Free();
            }
        }

        /// <summary>
        /// Creates a Managed NurbsCurve3D from a Native LN_NurbsCurve struct.
        /// Copies data immediately.
        /// </summary>
        public static NurbsCurve3D FromNativeTo3D(LN_NurbsCurve native)
        {
            if (native.control_point_count == 0)
                return null;

            // Copy Knots
            double[] knots = new double[native.knot_count];
            Marshal.Copy(native.knot_vector, knots, 0, native.knot_count);

            // Copy Control Points (XYZW -> Vector3)
            // Assumes non-rational or normalized if casting to 3D directly
            // For rigorous conversion, we should check Weights.

            // Marshalling arrays of structs is tricky with IntPtr.
            // We use pointer arithmetic or PtrToStructure in a loop.
            var points = new Vector3[native.control_point_count];
            int structSize = Marshal.SizeOf<XYZW>();
            IntPtr currentPtr = native.control_points;

            for (int i = 0; i < native.control_point_count; i++)
            {
                XYZW pt = Marshal.PtrToStructure<XYZW>(currentPtr);

                // Convert Homogeneous XYZW back to Euclidean Vector3
                if (Math.Abs(pt.w) > 1e-9)
                {
                    float invW = 1.0f / (float)pt.w;
                    points[i] = new Vector3(
                        (float)pt.wx * invW,
                        (float)pt.wy * invW,
                        (float)pt.wz * invW
                    );
                }
                else
                {
                    points[i] = new Vector3((float)pt.wx, (float)pt.wy, (float)pt.wz);
                }

                currentPtr += structSize; // Move pointer
            }

            return new NurbsCurve3D(native.degree, knots, points);
        }

        /// <summary>
        /// Creates a Managed RationalNurbsCurve3D from a Native LN_NurbsCurve struct.
        /// </summary>
        public static RationalNurbsCurve3D FromNativeToRational(LN_NurbsCurve native)
        {
            if (native.control_point_count == 0)
                return null;

            double[] knots = new double[native.knot_count];
            Marshal.Copy(native.knot_vector, knots, 0, native.knot_count);

            var points = new Vector4[native.control_point_count];
            int structSize = Marshal.SizeOf<XYZW>();
            IntPtr currentPtr = native.control_points;

            for (int i = 0; i < native.control_point_count; i++)
            {
                XYZW pt = Marshal.PtrToStructure<XYZW>(currentPtr);

                // Keep raw Homogeneous-ish data?
                // Wait, RationalNurbsCurve3D expects (X,Y,Z,W) Euclidean.
                // We need to divide by W for X,Y,Z but keep W.

                if (Math.Abs(pt.w) > 1e-9)
                {
                    float invW = 1.0f / (float)pt.w;
                    points[i] = new Vector4(
                        (float)pt.wx * invW,
                        (float)pt.wy * invW,
                        (float)pt.wz * invW,
                        (float)pt.w
                    );
                }
                else
                {
                    points[i] = new Vector4((float)pt.wx, (float)pt.wy, (float)pt.wz, (float)pt.w);
                }

                currentPtr += structSize;
            }

            return new RationalNurbsCurve3D(native.degree, knots, points);
        }
    }
}
