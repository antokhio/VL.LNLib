using System.Runtime.InteropServices;
using LNLibSharp;
using VL.LNLib.Base;
using VL.LNLib.Extensions;
using VL.LNLib.Helpers;

namespace VL.LNLib.Nurbs
{
    public record NurbsCurve<T> : Validatable, IDisposable
        where T : struct
    {
        private IReadOnlyList<T> _controlPoints;
        public IReadOnlyList<T> ControlPoints
        {
            get => _controlPoints;
            init
            {
                _controlPoints = value;
                Curve = null;
            }
        }
        private int _degree;
        public int Degree
        {
            get => _degree;
            init
            {
                _degree = value;
                Curve = null;
            }
        }

        private IReadOnlyList<float> _knots;
        public IReadOnlyList<float> Knots
        {
            get => _knots;
            init
            {
                _knots = value;
                Curve = null;
            }
        }

        private LN_NurbsCurve? _curve;
        public LN_NurbsCurve? Curve
        {
            get
            {
                if (_curve is null)
                {
                    Validate();

                    if (IsValid)
                    {
                        _curve = NurbsCurveHelper.Build(
                            ControlPoints,
                            Knots,
                            Degree,
                            ref _knotsHandle,
                            ref _cpsHanldle
                        );
                    }

                    return _curve;
                }

                return _curve;
            }
            set => _curve = value;
        }

        private GCHandle _knotsHandle;
        private GCHandle _cpsHanldle;

        public NurbsCurve(IReadOnlyList<T> controlPoints, int degree)
        {
            ControlPoints = controlPoints;
            Degree = degree;
            Knots = KnotsHelper.GenerateClampedKnots(degree, controlPoints.Count);
        }

        protected NurbsCurve(NurbsCurve<T> original)
            : base(original)
        {
            ControlPoints = original.ControlPoints;
            Degree = original.Degree;
            Knots = original.Knots;

            _knotsHandle = default;
            _cpsHanldle = default;
            _curve = null;
        }

        public NurbsCurve(LN_NurbsCurve curve)
        {
            // 1. Marshal Knots from native double* to managed float[]
            var kCount = curve.knot_count;
            var knotsDouble = new double[kCount];
            Marshal.Copy(curve.knot_vector, knotsDouble, 0, kCount);

            var knots = new float[kCount];
            for (int i = 0; i < kCount; i++)
            {
                knots[i] = (float)knotsDouble[i];
            }
            Knots = knots;

            // 2. Marshal Control Points from native XYZW* to managed T[]
            var cCount = curve.control_point_count;
            var controlPoints = new T[cCount];
            var ptr = curve.control_points;
            var stride = Marshal.SizeOf<XYZW>();

            for (int i = 0; i < cCount; i++)
            {
                // Read XYZW struct from pointer offset
                var xyzw = Marshal.PtrToStructure<XYZW>(ptr + (i * stride));
                // Convert to T (Vector2/3/4)
                controlPoints[i] = xyzw.FromXYZW<T>();
            }
            ControlPoints = controlPoints;

            // 3. Reset handles so this instance manages its own lifecycle
            _knotsHandle = default;
            _cpsHanldle = default;
            _curve = null;
        }

        public override void Validate()
        {
            VALIDATE_ARGUMENT(Degree > 0, nameof(Degree), "Degree must be greater than zero.");
            VALIDATE_ARGUMENT(
                Knots.Count > 0,
                nameof(Knots),
                "Knots size must be greater than zero"
            );
            VALIDATE_ARGUMENT(
                LNLibValidationUtils.IsValidKnotVector(Knots.ToDoubleArray(), Knots.Count) == 1,
                "knotVector",
                "KnotVector must be a nondecreasing sequence of real numbers."
            );
            VALIDATE_ARGUMENT(
                ControlPoints.Count > 0,
                "controlPoints",
                "ControlPoints must contain one point at least."
            );
            VALIDATE_ARGUMENT(
                LNLibValidationUtils.IsValidNurbs(Degree, Knots.Count, ControlPoints.Count) == 1,
                "controlPoints",
                "Arguments must be fit: m = n + p + 1"
            );
        }

        public void Dispose()
        {
            if (_knotsHandle.IsAllocated)
                _knotsHandle.Free();
            if (_cpsHanldle.IsAllocated)
                _cpsHanldle.Free();

            GC.SuppressFinalize(this);
        }
    }
}
