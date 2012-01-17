using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoDiff
{
    partial class ExpressionTreeDifferentiator
    {
        class EvalGenerator
        {
            private readonly MathMethods mathMethods;

            public EvalGenerator(MathMethods mathMethods)
            {
                this.mathMethods = mathMethods;
            }

            public Action<double[]> Generate(Compiled.TapeElement[] tape, int startIndex)
            {
                var tapeParameter = Expression.Parameter(typeof(double[]), "tape");
                var evalExpressions = from idx in Enumerable.Range(startIndex, tape.Length - startIndex)
                                      let tapeElement = tape[idx]
                                      select GetEvalExpression(tapeParameter, idx, tapeElement);

                var evalBlock = Expression.Block(evalExpressions);
                var evalMethodExpression = Expression.Lambda<Action<double[]>>(evalBlock, tapeParameter);
                var evalMethod = evalMethodExpression.Compile();

                return evalMethod;
            }

            private Expression GetEvalExpression(ParameterExpression tapeParameter, int tapeIndex, Compiled.TapeElement tapeElement)
            {
                var visitor = new EvalVisitor(mathMethods, tapeParameter, tapeIndex);
                tapeElement.Accept(visitor);
                return visitor.EvalExpression;
            }
        }

        class EvalVisitor : Compiled.ITapeVisitor
        {
            private readonly MathMethods mathMethods;
            private readonly ParameterExpression array;
            private readonly Expression currentElement;

            public EvalVisitor(MathMethods methods, ParameterExpression array, int index)
            {
                this.mathMethods = methods;
                this.array = array;
                this.currentElement = ArrayElement(index);
            }

            public Expression EvalExpression { get; private set; }

            public void Visit(Compiled.Constant elem)
            {
                var constantExpression = Expression.Constant(elem.Value);
                UpdateResult(constantExpression);
            }

            public void Visit(Compiled.Exp elem)
            {
                var arg = ArrayElement(elem.Arg);
                var exp = mathMethods.Exp(arg);

                UpdateResult(exp);
            }

            public void Visit(Compiled.Log elem)
            {
                var arg = ArrayElement(elem.Arg);
                var log = mathMethods.Log(arg);

                UpdateResult(log);
            }

            public void Visit(Compiled.ConstPower elem)
            {
                var powBase = ArrayElement(elem.Base);
                var exponent = Expression.Constant(elem.Exponent);
                var pow = mathMethods.Pow(powBase, exponent);

                UpdateResult(pow);
            }

            public void Visit(Compiled.TermPower elem)
            {
                var powBase = ArrayElement(elem.Base);
                var exponent = ArrayElement(elem.Exponent);
                var pow = mathMethods.Pow(powBase, exponent);

                UpdateResult(pow);
            }

            public void Visit(Compiled.Product elem)
            {
                var left = ArrayElement(elem.Left);
                var right = ArrayElement(elem.Right);
                var product = Expression.Multiply(left, right);

                UpdateResult(product);
            }

            public void Visit(Compiled.Sum elem)
            {
                var summandsQuery = from idx in elem.Terms
                                    select ArrayElement(idx);
                var summands = summandsQuery.ToArray();

                if (summands.Length == 0)
                    UpdateResult(Expression.Constant(0.0));
                else if (summands.Length == 1)
                    UpdateResult(summands[0]);
                else
                {
                    var sum = Expression.Add(summands[0], summands[1]);
                    for (int i = 2; i < summands.Length; ++i)
                        sum = Expression.Add(sum, summands[i]);
                    UpdateResult(sum);
                }
            }

            public void Visit(Compiled.Variable var)
            {
                UpdateResult(Expression.Empty());
            }

            public void Visit(Compiled.UnaryFunc elem)
            {
                throw new NotSupportedException();
            }

            public void Visit(Compiled.BinaryFunc elem)
            {
                throw new NotSupportedException();
            }

            public void Visit(Compiled.NaryFunc elem)
            {
                throw new NotSupportedException();
            }

            private void UpdateResult(Expression value)
            {
                EvalExpression = Expression.Assign(currentElement, value);
            }

            private Expression ArrayElement(int index)
            {
                var indexExpression = Expression.Constant(index);
                var valueExpression = Expression.ArrayAccess(array, indexExpression);
                return valueExpression;
            }
        }
    }
}
