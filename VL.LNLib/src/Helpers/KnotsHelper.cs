namespace VL.LNLib.Helpers
{
    public static class KnotsHelper
    {
        public static IReadOnlyList<float> GenerateClampedKnots(int degree, int controlPointCount)
        {
            // Formula: Count = ControlPoints + Degree + 1
            int knotCount = controlPointCount + degree + 1;
            var knots = new float[knotCount];

            // 1. Start with (Degree + 1) zeros
            for (int i = 0; i <= degree; i++)
                knots[i] = 0.0f;

            // 2. End with (Degree + 1) ones (or max value)
            // We usually normalize domain to [0..1] or [0..Count-Degree]
            // Standard approach: 0 to (Count - Degree)
            float maxVal = controlPointCount - degree;

            for (int i = knotCount - 1 - degree; i < knotCount; i++)
                knots[i] = maxVal;

            // 3. Fill the middle with simple increments
            int middleCount = knotCount - 2 * (degree + 1);
            for (int i = 0; i < middleCount; i++)
            {
                knots[degree + 1 + i] = i + 1.0f;
            }

            return knots;
        }
    }
}
