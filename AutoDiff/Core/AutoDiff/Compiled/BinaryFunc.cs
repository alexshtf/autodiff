using System;

namespace AutoDiff.Compiled
{
    internal sealed class BinaryFunc : TapeElement
    {
        private const int LeftIdx = 0;
        private const int RightIdx = 1;
        
        private TapeElement Left => Inputs.Element(LeftIdx);
        private TapeElement Right => Inputs.Element(RightIdx);

        private readonly Func<double, double, double> eval;
        private readonly Func<double, double, Tuple<double, double>> diff;

        public BinaryFunc(Func<double, double, double> eval, Func<double, double, Tuple<double, double>> diff)
        {
            this.eval = eval;
            this.diff = diff;
        }

        public override void Eval()
        {
            Value = eval(Left.Value, Right.Value);
        }

        public override void Diff()
        {
            var left = Left.Value;
            var right = Right.Value;
            
            Value = eval(left, right);
            var grad = diff(left, right);
            Inputs.SetWeight(LeftIdx, grad.Item1);
            Inputs.SetWeight(RightIdx, grad.Item2);
        }
    }
}
