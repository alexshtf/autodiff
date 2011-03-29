using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public partial class CompiledDifferentiator
    {
        private class EvalVisitor : Compiled.ITapeVisitor
        {
            public readonly Compiled.TapeElement[] tape;

            public EvalVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public void Visit(Compiled.Constant elem)
            {
            }

            public void Visit(Compiled.Exp elem)
            {
                elem.Value = Math.Exp(ValueOf(elem.Arg));
            }

            public void Visit(Compiled.Log elem)
            {
                elem.Value = Math.Log(ValueOf(elem.Arg));
            }

            public void Visit(Compiled.Power elem)
            {
                elem.Value = Math.Pow(ValueOf(elem.Base), elem.Exponent);
            }

            public void Visit(Compiled.Product elem)
            {
                elem.Value = ValueOf(elem.Left) * ValueOf(elem.Right);
            }

            public void Visit(Compiled.Sum elem)
            {
                elem.Value = 0;
                for (int i = 0; i < elem.Terms.Length; ++i)
                    elem.Value += ValueOf(elem.Terms[i]);
            }

            public void Visit(Compiled.Variable var)
            {
            }

            private double ValueOf(int index)
            {
                return tape[index].Value;
            }
        }
    }
}
