namespace VL.LNLib.Curve
{
    public abstract record BezierCurve<T>
    {
        public int ControlPointsResolution { get; set; }
        public int Degree => ControlPointsResolution - 1;
        public IReadOnlyList<T> ControlPoints { get; set; }

        public bool IsValid
        {
            get
            {
                if (Degree <= 0)
                    return false;
                if (ControlPoints == null || ControlPoints.Count == 0)
                    return false;
                return ControlPoints.Count == Degree + 1;
            }
        }

        protected BezierCurve(int controlPointsResolution, IReadOnlyList<T> controlPoints)
        {
            ControlPointsResolution = controlPointsResolution;
            ControlPoints = controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
        }

        public abstract T GetPointOnCurve(float t);
    }
}
