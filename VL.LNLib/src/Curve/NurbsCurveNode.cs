using Stride.Core.Mathematics;
using VL.Core.Import;
using VL.LNLib.Helpers;

namespace VL.LNLib.Curve
{
    [ProcessNode]
    public abstract class NurbsCurveNode<TCurve, TPoint>
        where TCurve : NurbsCurve<TPoint>
    {
        protected const int DefaultDegree = 1;

        private TCurve _output;
        public TCurve Output => _output;
        public bool IsValid => _output.IsValid;

        private IReadOnlyList<double>? _knots;

        public NurbsCurveNode(TCurve curve)
        {
            _output = curve;
        }

        public void SetDegree(int degree = DefaultDegree)
        {
            if (_output.Degree != degree)
            {
                _output = _output with
                {
                    Degree = degree,
                    Knots =
                        _knots
                        ?? KnotsHelper.GenerateClampedKnots(degree, _output.ControlPointCount),
                };
            }
        }

        public void SetControlPoints(IReadOnlyList<TPoint> controlPoints)
        {
            if (
                !EqualityComparer<IReadOnlyList<TPoint>>.Default.Equals(
                    _output.ControlPoints,
                    controlPoints
                )
            )
            {
                _output = _output with
                {
                    ControlPoints = controlPoints,
                    Knots =
                        _knots
                        ?? KnotsHelper.GenerateClampedKnots(_output.Degree, controlPoints.Count),
                };
            }
        }

        public void SetKnots(IReadOnlyList<double> knots)
        {
            // Logic:
            // 1. If 'knots' is null (pin disconnected), we switch '_knots' to null (Auto Mode).
            // 2. We then update '_output.Knots' to be either the new custom knots OR the auto-generated ones.
            if (!EqualityComparer<IReadOnlyList<double>>.Default.Equals(knots, _knots))
            {
                _knots = knots;

                var effectiveKnots =
                    _knots
                    ?? KnotsHelper.GenerateClampedKnots(
                        _output.Degree,
                        _output.ControlPoints.Count
                    );

                _output = _output with { Knots = effectiveKnots };
            }
        }
    }

    [ProcessNode(Name = "NurbsCurve (2D)")]
    public class NurbsCurve2DNode : NurbsCurveNode<NurbsCurve2D, Vector2>
    {
        protected static readonly IReadOnlyList<Vector2> DefaultControlPoints =
        [
            new(-0.5f, 0f),
            new(0.5f, 0f),
        ];

        protected static readonly IReadOnlyList<double> DefaultKnots =
            KnotsHelper.GenerateClampedKnots(DefaultDegree, DefaultControlPoints.Count);

        public NurbsCurve2DNode()
            : base(new(DefaultDegree, DefaultKnots, DefaultControlPoints)) { }
    }

    [ProcessNode(Name = "NurbsCurve (3D)")]
    public class NurbsCurve3DNode : NurbsCurveNode<NurbsCurve3D, Vector3>
    {
        protected static readonly IReadOnlyList<Vector3> DefaultControlPoints =
        [
            new(-0.5f, 0f, 0f),
            new(0.5f, 0f, 0f),
        ];

        protected static readonly IReadOnlyList<double> DefaultKnots =
            KnotsHelper.GenerateClampedKnots(DefaultDegree, DefaultControlPoints.Count);

        public NurbsCurve3DNode()
            : base(new(DefaultDegree, DefaultKnots, DefaultControlPoints)) { }
    }

    [ProcessNode(Name = "RationalNurbsCurve (3D)", Category = "LNLib.Curve")]
    public class RationalNurbsCurve3DNode : NurbsCurveNode<RationalNurbsCurve3D, Vector4>
    {
        protected static readonly IReadOnlyList<Vector4> DefaultControlPoints =
        [
            new(-0.5f, 0f, 0f, 1f),
            new(0.5f, 0f, 0f, 1f),
        ];

        protected static readonly IReadOnlyList<double> DefaultKnots =
            KnotsHelper.GenerateClampedKnots(DefaultDegree, DefaultControlPoints.Count);

        public RationalNurbsCurve3DNode()
            : base(new(DefaultDegree, DefaultKnots, DefaultControlPoints)) { }
    }
}
