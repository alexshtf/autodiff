using System;

namespace AutoDiff.Compiled
{
    internal sealed class BinaryFunc : TapeElement
    {
        private const int LeftIdx = 0;
        private const int RightIdx = 1;
        
        private int Left => Inputs.Index(LeftIdx);
        private int Right => Inputs.Index(RightIdx);

        private readonly Func<double, double, double> eval;
        private readonly Func<double, double, Tuple<double, double>> diff;

        public BinaryFunc(Func<double, double, double> eval, Func<double, double, Tuple<double, double>> diff)
        {
            this.eval = eval;
            this.diff = diff;
        }

        public override void Eval(TapeElement[] tape)
        {
            Value = eval(tape[Left].Value, tape[Right].Value);
        }

        public override void Diff(TapeElement[] tape)
        {
            var left = tape[Left].Value;
            var right = tape[Right].Value;
            
            Value = eval(left, right);
            var grad = diff(left, right);
            Inputs.SetWeight(LeftIdx, grad.Item1);
            Inputs.SetWeight(RightIdx, grad.Item2);
        }
    }
}
