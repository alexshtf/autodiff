using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace AutoDiff
{
    partial class ExpressionTreeDifferentiator
    {
        class DiffGenerator
        {
            private readonly MathMethods mathMethods;

            public DiffGenerator(MathMethods mathMethods)
            {
                this.mathMethods = mathMethods;
            }

            public Action<double[], double[]> Generate(Compiled.TapeElement[] tape)
            {
                var evalTapeParameter = Expression.Parameter(typeof(double[]), "evalTape");
                var diffTapeParameter = Expression.Parameter(typeof(double[]), "diffTape");

                var diffExpressions = new List<Expression>();
                for (int i = tape.Length - 2; i >= 0; --i)
                {
                    var currentDiffExpressions = GetDiffExpession(evalTapeParameter, diffTapeParameter, i, tape);
                    diffExpressions.AddRange(currentDiffExpressions);
                }

                var diffBlock = diffExpressions.Count > 0 ? (Expression)Expression.Block(diffExpressions) : (Expression)Expression.Empty();
                var diffMethodExpression = Expression.Lambda<Action<double[], double[]>>(diffBlock, evalTapeParameter, diffTapeParameter);
                var diffMethod = diffMethodExpression.Compile();

                return diffMethod;
            }

            private IEnumerable<Expression> GetDiffExpession(ParameterExpression evalTape, ParameterExpression diffTape, int i, Compiled.TapeElement[] tape)
            {
                Expression diffTapeElement = Expression.ArrayAccess(diffTape, Expression.Constant(i));
                Expression initZero = Expression.Assign(diffTapeElement, Expression.Constant(0.0));
                yield return initZero;

                for (int j = 0; j < tape[i].InputOf.Length; ++j)
                {
                    var connection = tape[i].InputOf[j];
                    Debug.Assert(connection.IndexOnTape > i);

                    var inputElement = tape[connection.IndexOnTape];
                    var diffVisitor = new DiffVisitor(mathMethods, evalTape, diffTape, connection.IndexOnTape, connection.ArgumentIndex);
                    inputElement.Accept(diffVisitor);

                    if (diffVisitor.LocalDerivative != null)
                        yield return Expression.AddAssign(diffTapeElement, diffVisitor.LocalDerivative);
                }
            }
        }

        private class DiffVisitor : Compiled.ITapeVisitor
        {
            private readonly MathMethods mathMethods;
            private readonly Expression evalTape;
            private readonly Expression diffTape;
            private readonly int elemIndex;
            private readonly int argIndex;
            private readonly Expression elemDiff;

            public DiffVisitor(MathMethods mathMethods, Expression evalTape, Expression diffTape, int elemIndex, int argIndex)
            {
                this.mathMethods = mathMethods;
                this.evalTape = evalTape;
                this.diffTape = diffTape;
                this.elemIndex = elemIndex;
                this.argIndex = argIndex;
                this.elemDiff = Expression.ArrayAccess(diffTape, Expression.Constant(elemIndex));
            }

            public Expression LocalDerivative { get; set; }

            public void Visit(Compiled.Constant elem)
            {
            }

            public void Visit(Compiled.Exp elem)
            {
                LocalDerivative = Expression.Multiply(elemDiff, ValueAt(elemIndex));
            }

            public void Visit(Compiled.Log elem)
            {
                LocalDerivative = Expression.Divide(elemDiff, ValueAt(elem.Arg));
            }

            public void Visit(Compiled.ConstPower elem)
            {
                var exponent = Expression.Constant(elem.Exponent);
                var exponentMinusOne = Expression.Subtract(exponent, Expression.Constant(1.0));
                var powerTerm = mathMethods.Pow(ValueAt(elem.Base), exponentMinusOne);
                var result = Expression.Multiply(Expression.Multiply(elemDiff, exponent), powerTerm);

                LocalDerivative = result;
            }

            public void Visit(Compiled.TermPower elem)
            {
                if (argIndex == 0)
                {
                    var baseVal = ValueAt(elem.Base);
                    var exponent = ValueAt(elem.Exponent);
                    var exponentMinusOne = Expression.Subtract(exponent, Expression.Constant(1.0));
                    var powerTerm = mathMethods.Pow(baseVal, exponentMinusOne);
                    var result = Expression.Multiply(Expression.Multiply(elemDiff, exponent), powerTerm);

                    LocalDerivative = result;
                }
                else
                {
                    var baseVal = ValueAt(elem.Base);
                    var exponent = ValueAt(elem.Exponent);

                    var result = Expression.Multiply(elemDiff, mathMethods.Pow(baseVal, exponent));
                    result = Expression.Multiply(result, mathMethods.Log(baseVal));

                    LocalDerivative = result;
                }
            }

            public void Visit(Compiled.Product elem)
            {
                if (argIndex == 0)
                    LocalDerivative = Expression.Multiply(elemDiff, ValueAt(elem.Right));
                else
                    LocalDerivative = Expression.Multiply(elemDiff, ValueAt(elem.Left));
            }

            public void Visit(Compiled.Sum elem)
            {
                LocalDerivative = elemDiff;
            }

            public void Visit(Compiled.Variable var)
            {
            }

            public void Visit(Compiled.UnaryFunc elem)
            {
                throw new NotImplementedException();
            }

            public void Visit(Compiled.BinaryFunc elem)
            {
                throw new NotImplementedException();
            }

            public void Visit(Compiled.NaryFunc elem)
            {
                throw new NotImplementedException();
            }

            private Expression ValueAt(int index)
            {
                var indexExpression = Expression.Constant(index);
                var valueExpression = Expression.ArrayIndex(evalTape, indexExpression);
                return valueExpression;
            }
        }
    }
}
