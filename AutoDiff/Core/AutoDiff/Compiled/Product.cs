namespace AutoDiff.Compiled
{
    internal sealed class Product : TapeElement
    {
        private const int LeftIdx = 0;
        private const int RightIdx = 1;
        private TapeElement Left => Inputs.Element(LeftIdx);
        private TapeElement Right => Inputs.Element(RightIdx);
        
        public override void Eval()
        {
            Value = Left.Value * Right.Value;
        }

        public override void Diff()
        {
            var left = Left.Value;
            var right = Right.Value;

            Value = left * right;
            Inputs.SetWeight(LeftIdx, right);
            Inputs.SetWeight(RightIdx, left);
        }
    }
}
