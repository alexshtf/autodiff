using System;

namespace AutoDiff
{
    internal partial class CompiledDifferentiator<T>
    {
        private sealed class EvalVisitor : Compiled.ITapeVisitor
        {
            private readonly Compiled.TapeElement[] tape;

            public EvalVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public override void Visit(Compiled.Constant elem)
            {
            }

            public override void Visit(Compiled.Exp elem)
            {
                elem.Value = Math.Exp(ValueOf(elem.Arg));
            }

            public override void Visit(Compiled.Log elem)
            {
                elem.Value = Math.Log(ValueOf(elem.Arg));
            }

            public override void Visit(Compiled.ConstPower elem)
            {
                elem.Value = Math.Pow(ValueOf(elem.Base), elem.Exponent);
            }

            public override void Visit(Compiled.TermPower elem)
            {
                elem.Value = Math.Pow(ValueOf(elem.Base), ValueOf(elem.Exponent));
            }

            public override void Visit(Compiled.Product elem)
            {
                elem.Value = ValueOf(elem.Left) * ValueOf(elem.Right);
            }

            public override void Visit(Compiled.Sum elem)
            {
                elem.Value = 0;
                var terms = elem.Terms;
                for(int i = 0; i < terms.Length; ++i)
                    elem.Value += ValueOf(terms[i]);
            }

            public override void Visit(Compiled.Variable var)
            {
            }

            public override void Visit(Compiled.UnaryFunc elem)
            {
                elem.Value = elem.Eval(ValueOf(elem.Arg));
            }

            public override void Visit(Compiled.BinaryFunc elem)
            {
                elem.Value = elem.Eval(ValueOf(elem.Left), ValueOf(elem.Right));
            }

            public override void Visit(Compiled.NaryFunc elem)
            {
                var args = new double[elem.Terms.Length];
                for (var i = 0; i < args.Length; i++)
                    args[i] = ValueOf(elem.Terms[i]);
                elem.Value = elem.Eval(args);
            }

            private double ValueOf(int index)
            {
                return tape[index].Value;
            }


        }
    }
}
