using Stride.Core.Mathematics;
using VL.Core.Import;
using VL.LNLib.Surface.VL.LNLib.Surface;

namespace VL.LNLib.Surface
{
    [ProcessNode]
    public abstract class BezierSurfaceNode<TSurface, TPoint>
        where TSurface : BezierSurface<TPoint>
    {
        protected const int DefaultControlPointsResolution = 2;

        private TSurface _output;
        public TSurface Output => _output;
        public bool IsValid => _output.IsValid;

        public BezierSurfaceNode(TSurface surface)
        {
            _output = surface;
        }

        public void SetControlPointsResolutionX(
            int controlPointsResolutionX = DefaultControlPointsResolution
        )
        {
            if (_output.ControlPointsResolutionX != controlPointsResolutionX)
            {
                _output = _output with { ControlPointsResolutionX = controlPointsResolutionX };
            }
        }

        public void SetControlPointsResolutionY(
            int controlPointsResolutionY = DefaultControlPointsResolution
        )
        {
            if (_output.ControlPointsResolutionY != controlPointsResolutionY)
            {
                _output = _output with { ControlPointsResolutionY = controlPointsResolutionY };
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

    [ProcessNode(Name = "BezierSurface (2D)")]
    public class BezierSurface2DNode : BezierSurfaceNode<BezierSurface2D, Vector2>
    {
        static readonly IReadOnlyList<Vector2> DefaultControlPoints =
        [
            new(-1, -1),
            new(1, -1),
            new(-1, 1),
            new(1, 1),
        ];

        public BezierSurface2DNode()
            : base(
                new(
                    DefaultControlPointsResolution,
                    DefaultControlPointsResolution,
                    DefaultControlPoints
                )
            ) { }
    }

    [ProcessNode(Name = "BezierSurface (3D)")]
    public class BezierSurface3DNode : BezierSurfaceNode<BezierSurface3D, Vector3>
    {
        // Default 1x1 linear patch (4 points)
        static readonly IReadOnlyList<Vector3> DefaultControlPoints =
        [
            new(0, 0, 0),
            new(1, 0, 0),
            new(0, 1, 0),
            new(1, 1, 0),
        ];

        public BezierSurface3DNode()
            : base(
                new(
                    DefaultControlPointsResolution,
                    DefaultControlPointsResolution,
                    DefaultControlPoints
                )
            ) { }
    }

    [ProcessNode(Name = "RationalBezierSurface (2D)")]
    public class RationalBezierSurface2DNode : BezierSurfaceNode<RationalBezierSurface2D, Vector3>
    {
        // X, Y, Weight
        static readonly IReadOnlyList<Vector3> DefaultControlPoints =
        [
            new(-1, -1, 1),
            new(1, -1, 1),
            new(-1, 1, 1),
            new(1, 1, 1),
        ];

        public RationalBezierSurface2DNode()
            : base(
                new(
                    DefaultControlPointsResolution,
                    DefaultControlPointsResolution,
                    DefaultControlPoints
                )
            ) { }
    }

    [ProcessNode(Name = "RationalBezierSurface (3D)")]
    public class RationalBezierSurface3DNode : BezierSurfaceNode<RationalBezierSurface3D, Vector4>
    {
        // Default 1x1 linear patch (4 points, weights = 1)
        static readonly IReadOnlyList<Vector4> DefaultControlPoints =
        [
            new(0, 0, 0, 1),
            new(1, 0, 0, 1),
            new(0, 1, 0, 1),
            new(1, 1, 0, 1),
        ];

        public RationalBezierSurface3DNode()
            : base(
                new(
                    DefaultControlPointsResolution,
                    DefaultControlPointsResolution,
                    DefaultControlPoints
                )
            ) { }
    }
}
