using System;

namespace AutoDiff.Compiled
{
    internal sealed class TermPower : TapeElement
    {
        private const int BaseIdx = 0;
        private const int ExpIdx = 1;
        private int Base => Inputs.Index(BaseIdx);
        private int Exponent => Inputs.Index(ExpIdx);

        public override void Eval(TapeElement[] tape)
        {
            Value = Math.Pow(tape[Base].Value, tape[Exponent].Value);
        }

        public override void Diff(TapeElement[] tape)
        {
            var baseVal = tape[Base].Value;
            var expVal = tape[Exponent].Value;

            Value = Math.Pow(baseVal, expVal);
            Inputs.SetWeight(BaseIdx, expVal * Math.Pow(baseVal, expVal - 1));
            Inputs.SetWeight(ExpIdx, Value * Math.Log(baseVal));
        }
    }
}
