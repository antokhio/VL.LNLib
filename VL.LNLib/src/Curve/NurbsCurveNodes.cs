//using VL.Core.Import;

//namespace VL.LNLib.Curve
//{
//    [ProcessNode()]
//    public class Reparametrize<TCurve, TPoint>
//        where TCurve : NurbsCurve<TPoint>
//    {
//        private TCurve? _curve;
//        private float _min = 0f;
//        private float _max = 1f;

//        public TCurve? Output => _curve;

//        public void SetCurve(TCurve? curve)
//        {
//            if (curve != _curve)
//            {
//                _curve = curve;

//                ReparametrizeCurve();
//            }
//        }

//        public void SetMinMax(float min = 0f, float max = 1f)
//        {
//            if (min != _min)
//            {
//                _min = min;

//                ReparametrizeCurve();
//            }
//        }

//        public void SetMax(float max = 1f)
//        {
//            if (max != _max)
//            {
//                _max = max;

//                ReparametrizeCurve();
//            }
//        }

//        private void ReparametrizeCurve()
//        {
//            if (_curve is null || _curve.IsValid == false)
//                return;

//            _curve.Reparametrize(_min, _max);
//        }
//    }

//    //[ProcessNode]
//    //public class GetPointsOnCurve { }
//}
