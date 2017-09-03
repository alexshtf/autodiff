using System;

namespace AutoDiff.Compiled
{
    internal sealed class Exp : TapeElement
    {
        private const int ArgIdx = 0;
        private TapeElement Arg => Inputs.Element(ArgIdx);
        
        public override void Eval()
        {
            Value = Math.Exp(Arg.Value);
        }

        public override void Diff()
        {
            Value = Math.Exp(Arg.Value);
            Inputs.SetWeight(ArgIdx, Value);        
        }
    }
}
