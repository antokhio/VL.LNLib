using Stride.Core.Mathematics;
using VL.Core.Import;

namespace VL.LNLib.Nurbs
{
    [ProcessNode]
    public abstract class NurbsCurveNode<TCurve, TPoint>
        where TCurve : NurbsCurve<TPoint>
        where TPoint : struct
    {
        private TCurve _output;
        public TCurve Output => _output;
        public bool IsValid => _output.IsValid;

        protected NurbsCurveNode(TCurve curve)
        {
            _output = curve;
        }

        public void SetDegree(int degree = 1)
        {
            if (_output.Degree != degree)
            {
                _output = _output with { Degree = degree };
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

    [ProcessNode(Name = "NurbsCurve (2D)")]
    public class NurbsCurve2DNode : NurbsCurveNode<NurbsCurve<Vector2>, Vector2>
    {
        static readonly NurbsCurve<Vector2> DefaultCurve = new([new(-0.5f, 0f), new(0.0f, 0f)], 1);

        public NurbsCurve2DNode()
            : base(DefaultCurve) { }
    }
}
