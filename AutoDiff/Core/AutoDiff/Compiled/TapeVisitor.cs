namespace AutoDiff.Compiled
{
    internal abstract class TapeVisitor
    {
        public abstract void Visit(Constant elem);
        public abstract void Visit(Exp elem);
        public abstract void Visit(Log elem);
        public abstract void Visit(ConstPower elem);
        public abstract void Visit(TermPower elem);
        public abstract void Visit(Product elem);
        public abstract void Visit(Sum elem);
        public abstract void Visit(Variable var);
        public abstract void Visit(UnaryFunc elem);
        public abstract void Visit(BinaryFunc elem);
        public abstract void Visit(NaryFunc elem);
    }
}
