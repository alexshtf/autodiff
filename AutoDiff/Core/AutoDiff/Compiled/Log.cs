using System;

namespace AutoDiff.Compiled
{
    internal sealed class Log : TapeElement
    {
        private const int ArgIdx = 0;
        private TapeElement Arg => Inputs.Element(ArgIdx);
        
        public override void Eval()
        {
            Value = Math.Log(Arg.Value);
        }

        public override void Diff()
        {
            var arg = Arg.Value;
            Value = Math.Log(arg);
            Inputs.SetWeight(ArgIdx, 1 / arg);        
        }
    }
}
