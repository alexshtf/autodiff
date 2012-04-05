using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoDiff
{
    partial class CompiledDifferentiator
    {
        private class ForwardSweepVisitor : Compiled.ITapeVisitor
        {
            private Compiled.TapeElement[] tape;

            public ForwardSweepVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public void Visit(Compiled.Constant elem)
            {
            }

            public void Visit(Compiled.Exp elem)
            {
                elem.Value = Math.Exp(ValueOf(elem.Arg));
                elem.Inputs[0].Weight = elem.Value;
            }

            public void Visit(Compiled.Log elem)
            {
                double arg = ValueOf(elem.Arg);
                elem.Value = Math.Log(arg);
                elem.Inputs[0].Weight = 1 / arg;
            }

            public void Visit(Compiled.ConstPower elem)
            {
                double baseVal = ValueOf(elem.Base);
                elem.Value = Math.Pow(baseVal, elem.Exponent);
                elem.Inputs[0].Weight = elem.Exponent * Math.Pow(baseVal, elem.Exponent - 1);
            }

            public void Visit(Compiled.TermPower elem)
            {
                double baseVal = ValueOf(elem.Base);
                double exponent = ValueOf(elem.Exponent);

                elem.Value = Math.Pow(baseVal, exponent);
                elem.Inputs[0].Weight = exponent * Math.Pow(baseVal, exponent - 1);
                elem.Inputs[1].Weight = elem.Value * Math.Log(baseVal);
            }

            public void Visit(Compiled.Product elem)
            {
                double left = ValueOf(elem.Left);
                double right = ValueOf(elem.Right);

                elem.Value = left * right;
                elem.Inputs[0].Weight = right;
                elem.Inputs[1].Weight = left;
            }

            public void Visit(Compiled.Sum elem)
            {
                elem.Value = 0;
                for (int i = 0; i < elem.Terms.Length; ++i)
                    elem.Value += ValueOf(elem.Terms[i]);

                for (int i = 0; i < elem.Inputs.Length; ++i)
                    elem.Inputs[i].Weight = 1;
            }

            public void Visit(Compiled.Variable var)
            {
            }

            public void Visit(Compiled.UnaryFunc elem)
            {
                double arg = ValueOf(elem.Arg);
                elem.Value = elem.Eval(arg);
                elem.Inputs[0].Weight = elem.Diff(arg);
            }

            public void Visit(Compiled.BinaryFunc elem)
            {
                double left = ValueOf(elem.Left);
                double right = ValueOf(elem.Right);

                elem.Value = elem.Eval(left, right);
                var grad = elem.Diff(left, right);
                elem.Inputs[0].Weight = grad.Item1;
                elem.Inputs[1].Weight = grad.Item2;
            }

            public void Visit(Compiled.NaryFunc elem)
            {
                double[] args = new double[elem.Terms.Length];
                for (int i = 0; i < args.Length; i++)
                    args[i] = ValueOf(elem.Terms[i]);

                elem.Value = elem.Eval(args);
                var grad = elem.Diff(args);
                for (int i = 0; i < grad.Length; ++i)
                    elem.Inputs[i].Weight = grad[i];
            }

            private double ValueOf(int index)
            {
                return tape[index].Value;
            }
        }
    }
}