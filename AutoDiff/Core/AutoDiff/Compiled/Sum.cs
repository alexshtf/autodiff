namespace AutoDiff.Compiled
{
    internal sealed class Sum : TapeElement
    {   
        public override void Eval()
        {
            Value = 0;
            for (var i = 0; i < Inputs.Length; ++i)
                Value += Inputs.Element(i).Value;
        }

        public override void Diff()
        {
            Value = 0;
            for (var i = 0; i < Inputs.Length; ++i)
                Value += Inputs.Element(i).Value;
        }
    }
}
