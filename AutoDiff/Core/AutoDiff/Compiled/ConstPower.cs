using System;

namespace AutoDiff.Compiled
{
    internal sealed class ConstPower : TapeElement
    {
        private const int BaseIdx = 0;        
        private TapeElement Base => Inputs.Element(BaseIdx);
        
        public double Exponent;

        public override void Eval()
        {
            Value = Math.Pow(Base.Value, Exponent);
        }

        public override void Diff()
        {
            var baseVal = Base.Value;
            Value = Math.Pow(baseVal, Exponent);
            Inputs.SetWeight(BaseIdx, Exponent * Math.Pow(baseVal, Exponent - 1));
        }
    }
}
