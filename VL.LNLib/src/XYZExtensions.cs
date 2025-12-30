using LNLibSharp;
using Stride.Core.Mathematics;
using VL.Lib.Collections;

namespace VL.LNLib
{
    public static class XYZExtensions
    {
        public static void FromVector(ref Vector3 input, out XYZ output)
        {
            var xyz = new XYZ();
            xyz.x = input.X;
            xyz.y = input.Y;
            xyz.z = input.Z;

            output = xyz;
        }

        public static void ToVector(ref XYZ input, out Vector3 output)
        {
            var vec = new Vector3();
            vec.X = (float)input.x;
            vec.Y = (float)input.y;
            vec.Z = (float)input.z;
            output = vec;
        }

        public static Vector3 GetPointOnCurveByBernstein(
            Spread<Vector3> controlPonits,
            int degree,
            float t
        )
        {
            var count = controlPonits.Count;
            if (count == 0)
                return new Vector3();

            var pool = System.Buffers.ArrayPool<XYZ>.Shared;
            var lnlibControlPoints = pool.Rent(count);

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var cp = controlPonits[i];
                    FromVector(ref cp, out lnlibControlPoints[i]);
                }

                var result = LNLibBezierCurve.GetPointOnCurveByBernstein(
                    degree,
                    lnlibControlPoints,
                    count,
                    t
                );

                ToVector(ref result, out Vector3 finalResult);
                return finalResult;
            }
            finally
            {
                pool.Return(lnlibControlPoints);
            }
        }
    }
}
