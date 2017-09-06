namespace AutoDiff.Compiled
{
    internal sealed class Sum : TapeElement
    {   
        public override void Eval(TapeElement[] tape)
        {
            Value = 0;
            for (var i = 0; i < Inputs.Length; ++i)
                Value += tape[Inputs.Index(i)].Value;
        }

        public override void Diff(TapeElement[] tape)
        {
            Value = 0;
            for (var i = 0; i < Inputs.Length; ++i)
                Value += tape[Inputs.Index(i)].Value;
        }
    }
}
