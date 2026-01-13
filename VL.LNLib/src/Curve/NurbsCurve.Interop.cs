//using System.Runtime.InteropServices;
//using LNLibSharp;
//using Stride.Core.Mathematics;

//namespace VL.LNLib.Curve
//{
//    public abstract partial record NurbsCurve<T> : IDisposable
//    {
//        private LN_NurbsCurve? _nativeCurve;
//        protected LN_NurbsCurve NativeCurve
//        {
//            get
//            {
//                if (_nativeCurve is null)
//                    Build();
//                return _nativeCurve ?? throw new ArgumentOutOfRangeException();
//            }
//        }

//        private GCHandle _knotsHandle;
//        private GCHandle _cpHandle;

//        protected virtual void Build()
//        {
//            if (this is null || this.IsValid == false)
//                throw new ArgumentException("Invalid NurbsCurve");

//            // Prepare Arrays
//            var knotsArray = this.Knots.ToArray();
//            var cpArray = this.ToNativeControlPoints();

//            var knotsHandle = GCHandle.Alloc(knotsArray, GCHandleType.Pinned);
//            var cpHandle = GCHandle.Alloc(cpArray, GCHandleType.Pinned);

//            try
//            {
//                _nativeCurve = new LN_NurbsCurve
//                {
//                    degree = this.Degree,
//                    knot_vector = knotsHandle.AddrOfPinnedObject(),
//                    knot_count = knotsArray.Length,
//                    control_points = cpHandle.AddrOfPinnedObject(),
//                    control_point_count = cpArray.Length,
//                };

//                _knotsHandle = knotsHandle;
//                _cpHandle = cpHandle;
//            }
//            catch
//            {
//                if (knotsHandle.IsAllocated)
//                    knotsHandle.Free();
//                if (cpHandle.IsAllocated)
//                    cpHandle.Free();

//                throw new ArgumentOutOfRangeException(
//                    "Failed to build LN_NurbsCurve from NurbsCurve"
//                );
//            }
//        }

//        public virtual XYZW[] ToNativeControlPoints()
//        {
//            var count = ControlPoints.Count;
//            var arr = new XYZW[count];

//            for (int i = 0; i < count; i++)
//            {
//                var v = ControlPoints[i];
//                arr[i] = v switch
//                {
//                    Vector2 vec2 => new XYZW
//                    {
//                        wx = vec2.X,
//                        wy = vec2.Y,
//                        wz = 0.0,
//                        w = 1.0,
//                    },
//                    Vector3 vec3 => new XYZW
//                    {
//                        wx = vec3.X,
//                        wy = vec3.Y,
//                        wz = vec3.Z,
//                        w = 1.0,
//                    },
//                    Vector4 vec4 => new XYZW
//                    {
//                        wx = vec4.X * vec4.W,
//                        wy = vec4.Y * vec4.W,
//                        wz = vec4.Z * vec4.W,
//                        w = vec4.W,
//                    },
//                    _ => throw new NotSupportedException(
//                        $"Control point type {typeof(T)} is not supported."
//                    ),
//                };
//            }

//            return arr;
//        }

//        public void Dispose()
//        {
//            if (_knotsHandle.IsAllocated)
//                _knotsHandle.Free();
//            if (_cpHandle.IsAllocated)
//                _cpHandle.Free();

//            GC.SuppressFinalize(this);
//        }
//    }
//}
