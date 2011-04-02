using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoDiff
{
    partial class CompiledDifferentiator
    {
        private class DiffVisitor : Compiled.ITapeVisitor
        {
            private readonly Compiled.TapeElement[] tape;
            public double LocalDerivative;
            public int ArgumentIndex;

            public DiffVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public void Visit(Compiled.Constant elem)
            {

            }

            public void Visit(Compiled.Exp elem)
            {
                LocalDerivative = elem.Derivative * elem.Value;
            }

            public void Visit(Compiled.Log elem)
            {
                LocalDerivative = elem.Derivative / ValueOf(elem.Arg);
            }

            public void Visit(Compiled.Power elem)
            {
                LocalDerivative = elem.Derivative * elem.Exponent * Math.Pow(ValueOf(elem.Base), elem.Exponent - 1);
            }

            public void Visit(Compiled.Product elem)
            {
                Debug.Assert(ArgumentIndex == 0 || ArgumentIndex == 1);
                if (ArgumentIndex == 0)
                    LocalDerivative = elem.Derivative * ValueOf(elem.Right);
                else
                    LocalDerivative = elem.Derivative * ValueOf(elem.Left);
            }

            public void Visit(Compiled.Sum elem)
            {
                LocalDerivative = elem.Derivative;
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
