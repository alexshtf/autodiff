namespace AutoDiff.Compiled
{
    internal sealed class Product : TapeElement
    {
        private const int LeftIdx = 0;
        private const int RightIdx = 1;
        private int Left => Inputs.Index(LeftIdx);
        private int Right => Inputs.Index(RightIdx);
        
        public override void Eval(TapeElement[] tape)
        {
            Value = tape[Left].Value * tape[Right].Value;
        }

        public override void Diff(TapeElement[] tape)
        {
            var left = tape[Left].Value;
            var right = tape[Right].Value;

            Value = left * right;
            Inputs.SetWeight(LeftIdx, right);
            Inputs.SetWeight(RightIdx, left);
        }
    }
}
