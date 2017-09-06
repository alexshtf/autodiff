using System;

namespace AutoDiff.Compiled
{
    internal sealed class Exp : TapeElement
    {
        private const int ArgIdx = 0;
        private int Arg => Inputs.Index(ArgIdx);
        
        public override void Eval(TapeElement[] tape)
        {
            Value = Math.Exp(tape[Arg].Value);
        }

        public override void Diff(TapeElement[] tape)
        {
            Value = Math.Exp(tape[Arg].Value);
            Inputs.SetWeight(ArgIdx, Value);        
        }
    }
}
