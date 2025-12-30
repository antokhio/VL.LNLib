using Stride.Core.Mathematics;
using VL.Core.Import;

namespace VL.LNLib.Curve
{
    [ProcessNode]
    public abstract class BezierCurveNode<TCurve, TPoint>
        where TCurve : BezierCurve<TPoint>
    {
        protected const int DefaultControlPointsResolution = 1;

        private TCurve _output;
        public TCurve Output => _output;
        public bool IsValid => _output.IsValid;

        public BezierCurveNode(TCurve curve)
        {
            _output = curve;
        }

        public void SetControlPointsResolution(
            int controlPointsResolution = DefaultControlPointsResolution
        )
        {
            if (_output.ControlPointsResolution != controlPointsResolution)
            {
                _output = _output with { ControlPointsResolution = controlPointsResolution };
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
                _output = _output with { ControlPoints = controlPoints };
            }
        }
    }

    [ProcessNode(Name = "BezierCurve (2D)")]
    public class BezierCurve2DNode : BezierCurveNode<BezierCurve2D, Vector2>
    {
        static readonly IReadOnlyList<Vector2> DefaultControlPoints = [new(-1, 0), new(1, 0)];

        public BezierCurve2DNode()
            : base(new(DefaultControlPointsResolution, DefaultControlPoints)) { }
    }

    [ProcessNode(Name = "BezierCurve (3D)")]
    public class BezierCurve3DNode : BezierCurveNode<BezierCurve3D, Vector3>
    {
        static readonly IReadOnlyList<Vector3> DefaultControlPoints = [new(-1, 0, 0), new(1, 0, 0)];

        public BezierCurve3DNode()
            : base(new(DefaultControlPointsResolution, DefaultControlPoints)) { }
    }

    [ProcessNode(Name = "RationalBezierCurve (2D)")]
    public class RationalBezierCurve2DNode : BezierCurveNode<RationalBezierCurve2D, Vector3>
    {
        static readonly IReadOnlyList<Vector3> DefaultControlPoints = [new(-1, 0, 1), new(1, 0, 1)];

        public RationalBezierCurve2DNode()
            : base(new(DefaultControlPointsResolution, DefaultControlPoints)) { }
    }

    [ProcessNode(Name = "RationalBezierCurve (3D)")]
    public class RationalBezierCurve3DNode : BezierCurveNode<RationalBezierCurve3D, Vector4>
    {
        static readonly IReadOnlyList<Vector4> DefaultControlPoints =
        [
            new(-1, 0, 0, 1),
            new(1, 0, 0, 1),
        ];

        public RationalBezierCurve3DNode()
            : base(new(DefaultControlPointsResolution, DefaultControlPoints)) { }
    }
}
