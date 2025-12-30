using Stride.Core.Mathematics;
using VL.Core.Import;

namespace VL.LNLib
{
    public enum PositioningMode
    {
        Absolute,
        Relative,
    }

    public record struct BezierSurface
    {
        public Int2 ControlPointsResolution { get; set; }
        public IReadOnlyList<Vector2> ControlPoints { get; set; }
        public PositioningMode Positioning { get; set; }

        //public static void Join(
        //    ref Int2 controlPointsResolution,
        //    ref IReadOnlyList<Vector2> controlPoints,
        //    PositioningMode positioning,
        //    out BezierSurface bezierSurface
        //) { }

        public static BezierSurface Default()
        {
            return new BezierSurface
            {
                ControlPointsResolution = new Int2(2, 2),
                ControlPoints = new List<Vector2>
                {
                    new Vector2(-1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, -1),
                    new Vector2(-1, -1),
                },
                Positioning = PositioningMode.Relative,
            };
        }
    }

    [ProcessNode(Name = "BezierSurface")]
    public class BezierSurfaceNode
    {
        private BezierSurface _bezierSurface = BezierSurface.Default();

        private bool _isValid = true;

        public void SetControlPoints(Int2 resolution, IReadOnlyList<Vector2> contrloPoints)
        {
            if (resolution.X < 2 || resolution.Y < 2) { }

            if (contrloPoints.Count != resolution.X * resolution.Y)
                throw new ArgumentException(
                    "ControlPoints count does not match ControlPointsResolution."
                );

            if (resolution != _bezierSurface.ControlPointsResolution)
            {
                _bezierSurface = _bezierSurface with { ControlPointsResolution = resolution };
            }
        }
    }
}
