using System;

namespace AutoDiff.Compiled
{
    internal sealed class ConstPower : TapeElement
    {
        private const int BaseIdx = 0;        
        private int Base => Inputs.Index(BaseIdx);
        
        public double Exponent;

        public override void Eval(TapeElement[] tape)
        {
            Value = Math.Pow(tape[Base].Value, Exponent);
        }

        public override void Diff(TapeElement[] tape)
        {
            var baseVal = tape[Base].Value;
            Value = Math.Pow(baseVal, Exponent);
            Inputs.SetWeight(BaseIdx, Exponent * Math.Pow(baseVal, Exponent - 1));
        }
    }
}
