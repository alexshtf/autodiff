using System;

namespace AutoDiff.Compiled
{
    internal sealed class TermPower : TapeElement
    {
        private const int BaseIdx = 0;
        private const int ExpIdx = 1;
        private TapeElement Base => Inputs.Element(BaseIdx);
        private TapeElement Exponent => Inputs.Element(ExpIdx);

        public override void Eval()
        {
            Value = Math.Pow(Base.Value, Exponent.Value);
        }

        public override void Diff()
        {
            var baseVal = Base.Value;
            var expVal = Exponent.Value;

            Value = Math.Pow(baseVal, expVal);
            Inputs.SetWeight(BaseIdx, expVal * Math.Pow(baseVal, expVal - 1));
            Inputs.SetWeight(ExpIdx, Value * Math.Log(baseVal));
        }
    }
}
