using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AutoDiff
{
    partial class ExpressionTreeDifferentiator : ICompiledTerm
    {
        private int tapeSize;
        private Action<double[]> evaluateTapeWithoutVariables;
        private Action<double[], double[]> diffTapeAction;

        public ExpressionTreeDifferentiator(Term term, Variable[] variables, Compiled.Compiler tapeCompiler)
        {
            Variables = Array.AsReadOnly(variables.ToArray());
            var evalGenerator = new EvalGenerator(new MathMethods());
            var diffGenerator = new DiffGenerator(new MathMethods());
            
            var tape = tapeCompiler.Compile(term, variables);
            tapeSize = tape.Length;
            evaluateTapeWithoutVariables = evalGenerator.Generate(tape, Variables.Count);
            diffTapeAction = diffGenerator.Generate(tape);
        }

        public double Evaluate(params double[] arg)
        {
            var tape = new double[tapeSize];
            EvaluateTape(arg, tape);
            return tape.Last();
        }

        public Tuple<double[], double> Differentiate(params double[] arg)
        {
            var evalTape = new double[tapeSize];
            EvaluateTape(arg, evalTape);

            var diffTape = new double[tapeSize];
            DifferentiateTape(evalTape, diffTape);

            var gradient = diffTape.Take(Dimension).ToArray();
            var value = evalTape.Last();

            return Tuple.Create(gradient, value);
        }

        public ReadOnlyCollection<Variable> Variables { get; private set; }

        private int Dimension
        {
            get { return Variables.Count; }
        }

        private void EvaluateTape(double[] arg, double[] tape)
        {
            for (int i = 0; i < Dimension; ++i)
                tape[i] = arg[i];

            evaluateTapeWithoutVariables(tape);
        }

        private void DifferentiateTape(double[] evalTape, double[] diffTape)
        {
            diffTape[tapeSize - 1] = 1;
            diffTapeAction(evalTape, diffTape);
        }
    }
}
