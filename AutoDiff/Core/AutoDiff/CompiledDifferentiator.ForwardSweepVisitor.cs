using System;

namespace AutoDiff
{
    internal partial class CompiledDifferentiator<T>
    {
        private sealed class ForwardSweepVisitor : Compiled.ITapeVisitor
        {
            private readonly Compiled.TapeElement[] tape;

            public ForwardSweepVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public override void Visit(Compiled.Constant elem)
            {
            }

            public override void Visit(Compiled.Exp elem)
            {
                elem.Value = Math.Exp(ValueOf(elem.Arg));
                elem.Inputs.SetWeight(0, elem.Value);
            }

            public override void Visit(Compiled.Log elem)
            {
                var arg = ValueOf(elem.Arg);
                elem.Value = Math.Log(arg);
                elem.Inputs.SetWeight(0, 1 / arg);
            }

            public override void Visit(Compiled.ConstPower elem)
            {
                var baseVal = ValueOf(elem.Base);
                elem.Value = Math.Pow(baseVal, elem.Exponent);
                elem.Inputs.SetWeight(0, elem.Exponent * Math.Pow(baseVal, elem.Exponent - 1));
            }

            public override void Visit(Compiled.TermPower elem)
            {
                var baseVal = ValueOf(elem.Base);
                var exponent = ValueOf(elem.Exponent);

                elem.Value = Math.Pow(baseVal, exponent);
                elem.Inputs.SetWeight(0, exponent * Math.Pow(baseVal, exponent - 1));
                elem.Inputs.SetWeight(1, elem.Value * Math.Log(baseVal));
            }

            public override void Visit(Compiled.Product elem)
            {
                var left = ValueOf(elem.Left);
                var right = ValueOf(elem.Right);

                elem.Value = left * right;
                elem.Inputs.SetWeight(0, right);
                elem.Inputs.SetWeight(1, left);
            }

            public override void Visit(Compiled.Sum elem)
            {
                elem.Value = 0;
                var terms = elem.Terms;
                for(var i = 0; i < terms.Length; ++i)
                    elem.Value += ValueOf(terms[i]);
            }

            public override void Visit(Compiled.Variable var)
            {
            }

            public override void Visit(Compiled.UnaryFunc elem)
            {
                double arg = ValueOf(elem.Arg);
                elem.Value = elem.Eval(arg);
                elem.Inputs.SetWeight(0, elem.Diff(arg));
            }

            public override void Visit(Compiled.BinaryFunc elem)
            {
                var left = ValueOf(elem.Left);
                var right = ValueOf(elem.Right);

                elem.Value = elem.Eval(left, right);
                var grad = elem.Diff(left, right);
                elem.Inputs.SetWeight(0, grad.Item1);
                elem.Inputs.SetWeight(1, grad.Item2);
            }

            public override void Visit(Compiled.NaryFunc elem)
            {
                var terms = elem.Terms;
                var args = new double[terms.Length];
                for (var i = 0; i < args.Length; i++)
                    args[i] = ValueOf(terms[i]);

                elem.Value = elem.Eval(args);
                var grad = elem.Diff(args);
                for (var i = 0; i < grad.Length; ++i)
                    elem.Inputs.SetWeight(i, grad[i]);
            }

            private double ValueOf(int index)
            {
                return tape[index].Value;
            }
        }
    }
}
