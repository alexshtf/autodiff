using System;

namespace AutoDiff.Compiled
{
    internal sealed class Log : TapeElement
    {
        private const int ArgIdx = 0;
        private int Arg => Inputs.Index(ArgIdx);
        
        public override void Eval(TapeElement[] tape)
        {
            Value = Math.Log(tape[Arg].Value);
        }

        public override void Diff(TapeElement[] tape)
        {
            var arg = tape[Arg].Value;
            Value = Math.Log(arg);
            Inputs.SetWeight(ArgIdx, 1 / arg);        
        }
    }
}
