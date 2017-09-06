using System;

namespace AutoDiff.Compiled
{
    internal sealed class UnaryFunc : TapeElement
    {
        private const int ArgIdx = 0;
        private int Arg => Inputs.Index(ArgIdx);
        
        private readonly Func<double, double> eval;
        private readonly Func<double, double> diff;

        public UnaryFunc(Func<double, double> eval, Func<double, double> diff)
        {
            this.eval = eval;
            this.diff = diff;
        }

        public override void Eval(TapeElement[] tape)
        {
            Value = eval(tape[Arg].Value);
        }

        public override void Diff(TapeElement[] tape)
        {
            var arg = tape[Arg].Value;
            Value = eval(arg);
            Inputs.SetWeight(ArgIdx, diff(arg));
        }
    }
}