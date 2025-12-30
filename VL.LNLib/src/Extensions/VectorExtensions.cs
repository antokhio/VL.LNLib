using LNLibSharp;
using Stride.Core.Mathematics;

namespace VL.LNLib.Extensions
{
    public static class VectorExtensions
    {
        public static XYZ ToXYZ(this Vector3 v) =>
            new XYZ
            {
                x = v.X,
                y = v.Y,
                z = v.Z,
            };

        public static Vector3 ToVector3(this XYZ xyz) =>
            new Vector3((float)xyz.x, (float)xyz.y, (float)xyz.z);

        public static XYZW ToXYZW(this Vector4 v) =>
            new XYZW
            {
                wx = v.X,
                wy = v.Y,
                wz = v.Z,
                w = v.W,
            };

        public static Vector4 ToVector4(this XYZW xyzw) =>
            new Vector4((float)xyzw.wx, (float)xyzw.wy, (float)xyzw.wz, (float)xyzw.w);

        /// <summary>
        /// Converts a 3D point to Homogeneous XYZW with Weight = 1.0.
        /// </summary>
        public static XYZW ToHomogeneousXYZW(this Vector3 v) =>
            new XYZW
            {
                wx = v.X,
                wy = v.Y,
                wz = v.Z,
                w = 1.0,
            };

        /// <summary>
        /// Converts a Euclidean Vector4 (X,Y,Z,W) to Homogeneous XYZW (X*W, Y*W, Z*W, W).
        /// </summary>
        public static XYZW ToHomogeneousXYZW(this Vector4 v)
        {
            double w = v.W;
            return new XYZW
            {
                wx = v.X * w,
                wy = v.Y * w,
                wz = v.Z * w,
                w = w,
            };
        }
    }
}
