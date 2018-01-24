using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static AutoDiff.Tests.Utils;
using static AutoDiff.TermBuilder;

namespace AutoDiff.Tests
{
    public class BasicDifferentiationTests
    {
        [Fact]
        public void DiffZero()
        {
            var zero = Constant(0);
            var v = new Variable();
            var grad = zero.Differentiate(Vec(v), NumVec(12));
            Assert.Equal(NumVec(0), grad);
        }

        [Fact]
        public void DiffConstant()
        {
            var c = Constant(100);
            var v = new Variable();
            var grad = c.Differentiate(Vec(v), NumVec(12));
            Assert.Equal(NumVec(0), grad);
        }

        [Fact]
        public void DiffVar()
        {
            var v = new Variable();
            var grad = v.Differentiate(Vec(v), NumVec(12));
            Assert.Equal(NumVec(1), grad);
        }

        [Fact]
        public void DiffPartialVars()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();
            var grad = y.Differentiate(Vec(x, y, z), NumVec(1, 2, 3));
            Assert.Equal(NumVec(0, 1, 0), grad);
        }

        [Fact]
        public void DiffProdTwoVars()
        {
            var x = new Variable();
            var y = new Variable();
            var func = x * y;
            var grad = func.Differentiate(Vec(x, y), NumVec(3, -4));
            Assert.Equal(NumVec(-4, 3), grad);
        }

        [Fact]
        public void DiffProdVarsConst()
        {
            var x = new Variable();
            var y = new Variable();
            var func = -3 * x * y;
            var grad = func.Differentiate(Vec(x, y), NumVec(2, -3));
            Assert.Equal(NumVec(9, -6), grad);
        }

        [Fact]
        public void DiffQuadraticSingleVar()
        {
            var x = new Variable();

            // f(x) = 2x²
            var func = 2 * Power(x, 2);
            var grad = func.Differentiate(Vec(x), NumVec(2));

            // df(x) = 4x
            Assert.Equal(NumVec(8), grad);
        }

        [Fact]
        public void DiffQuadraticTwoVars()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = 2x² - 3y²
            var func = 2 * Power(x, 2) - 3 * Power(y, 2);
            var grad = func.Differentiate(Vec(x, y), NumVec(2, 3));
            
            // df(x, y) = (4x, -6y)
            Assert.Equal(NumVec(8, -18), grad);
        }

        [Fact]
        public void DiffMeanSquaredError()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = (x - 5)² + (x + 2)²
            var func = 0.5 * (Power(x - 5, 2) + Power(y + 2, 2));
            var gradOpt = func.Differentiate(Vec(x, y), NumVec(5, -2));
            var gradGen = func.Differentiate(Vec(x, y), NumVec(3, 1));

            // df(x, y) = [x-5, x+2]
            Assert.Equal(NumVec(0, 0), gradOpt);
            Assert.Equal(NumVec(-2, 3), gradGen);
        }

        [Fact]
        public void DiffRational()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = (x² - xy + y²) / (x + y)
            var func = (Power(x, 2) - x * y + Power(y, 2)) * Power(x + y, -1);

            // df(1,4) = [-0.92, 0.88]
            var grad1 = func.Differentiate(Vec(x, y), NumVec(1, 4));
            grad1[0] = Math.Round(grad1[0], 2);
            grad1[1] = Math.Round(grad1[1], 2);
            Assert.Equal(NumVec(-0.92, 0.88), grad1);

            // df(-6,4) = [-11, -26]
            var grad2 = func.Differentiate(Vec(x, y), NumVec(-6, 4));
            Assert.Equal(NumVec(-11, -26), grad2);
        }

        [Fact]
        public void DiffPolynomial1()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();

            // f(x,y,z) = 2(x-y)² + 5xy - 3y²
            var func = 2 * Power(x - y, 2) + 5 * x * y - 3 * Power(y, 2);
            var diff = func.Compile(x, y, z);

            var result = diff.Differentiate(NumVec(1, 2, -3));
            Assert.Equal(NumVec(6, -3, 0), result.Item1);
            Assert.Equal(0, result.Item2);
        }

        [Fact]
        public void DiffPolynomial2()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();

            var terms = new Term[] 
                { 
                    x + 2 * Power(x - y, 2), // x + 2(x-y)²
                    x*y - y*z,               // xy - yz
                    3 * x * y * z,           // 3xyz
                };

            // (x + 2(x-y)²)², (xy - yz)², (3xyz)²
            terms = terms.Select(t => Power(t, 2)).ToArray();

            // 0.25 * ((x + 2(x-y)²)² + (xy - yz)² + (3xyz)²) + (y - x + 1)²
            var func = 0.25 * Sum(terms) + Power(y - x + 1, 2);

            var diff = func.Compile(x, y, z);
            var result = diff.Differentiate(1, 2, -3);

            // asserts checked with MATLAB
            Assert.Equal(NumVec(161.5, 107, -62), result.Item1);
            Assert.Equal(103.25, result.Item2);
        }

        [Fact]
        public void DiffExp()
        {
            var x = new Variable();
            var func = Exp(x);

            var grad1 = func.Differentiate(Vec(x), NumVec(1));
            var grad2 = func.Differentiate(Vec(x), NumVec(-2));

            Assert.Equal(NumVec(Math.Exp(1)), grad1);
            Assert.Equal(NumVec(Math.Exp(-2)), grad2);
        }

        [Fact]
        public void DiffTermPower()
        {
            var x = new Variable();
            var y = new Variable();
            var func = Power(x, y);

            var grad = func.Differentiate(Vec(x, y), NumVec(2, 3));

            Assert.Equal(NumVec(12, 8 * Math.Log(2)), grad);
        }

        [Fact]
        public void DiffTermPowerSingleVariable()
        {
            var x = new Variable();
            var func = Power(x, x);

            var grad = func.Differentiate(Vec(x), NumVec(2.5));
            var expectedGrad = NumVec(Math.Pow(2.5, 2.5) * (Math.Log(2.5) + 1));
            Assert.Equal(expectedGrad[0], grad[0], 10);
        }

        [Fact]
        public void DiffUnarySimple()
        {
            var v = new Variable();

            Func<double, double> eval = x => x * x;
            Func<double, double> diff = x => 2 * x;

            var term = new UnaryFunc(eval, diff, v);

            var y1 = term.Differentiate(Vec(v), Vec(1.0)); // 2
            var y2 = term.Differentiate(Vec(v), Vec(2.0)); // 4
            var y3 = term.Differentiate(Vec(v), Vec(3.0)); // 6

            Assert.Equal(Vec(2.0), y1);
            Assert.Equal(Vec(4.0), y2);
            Assert.Equal(Vec(6.0), y3);
        }

        [Fact]
        public void DiffUnaryComplex()
        {
            var square = UnaryFunc.Factory(x => x * x, x => 2 * x);

            // f(x, y) = x^2 + 2 * y^2
            // df/dx = 2 * x
            // df/dy = 4 * y
            var v = Vec(new Variable(), new Variable());
            var term = square(v[0]) + 2 * square(v[1]);

            var y1 = term.Differentiate(v, Vec(1.0, 0.0));  // (2.0, 0.0)
            var y2 = term.Differentiate(v, Vec(0.0, 1.0));  // (0.0, 4.0)
            var y3 = term.Differentiate(v, Vec(2.0, 1.0));  // (4.0, 4.0)

            Assert.Equal(Vec(2.0, 0.0), y1);
            Assert.Equal(Vec(0.0, 4.0), y2);
            Assert.Equal(Vec(4.0, 4.0), y3);
        }

        [Fact]
        public void DiffBinarySimple()
        {
            var v = Vec(new Variable(), new Variable());
            var func = BinaryFunc.Factory(
                (x, y) => x * x - x * y,
                (x, y) => Tuple.Create(2 * x - y, -x));

            // f(x, y) = x² - xy
            // df/dx = 2x - y
            // df/dy = -x
            var term = func(v[0], v[1]);

            var y1 = term.Differentiate(v, Vec(1.0, 0.0)); // (2, -1)
            var y2 = term.Differentiate(v, Vec(0.0, 1.0)); // (-1, 0)
            var y3 = term.Differentiate(v, Vec(1.0, 2.0)); // (0, -1)

            Assert.Equal(Vec(2.0, -1.0), y1);
            Assert.Equal(Vec(-1.0, 0.0), y2);
            Assert.Equal(Vec(0.0, -1.0), y3);
        }

        [Fact]
        public void DiffBinaryComplex()
        {
            var v = Vec(new Variable(), new Variable());
            var func = BinaryFunc.Factory(
                (x, y) => x * x - x * y,
                (x, y) => Tuple.Create(2 * x - y, -x));

            // f(x, y) = x² - xy - y² + xy = x² - y²
            // df/dx = 2x
            // df/dy = -2y
            var term = func(v[0], v[1]) - func(v[1], v[0]);

            var y1 = term.Differentiate(v, Vec(1.0, 0.0)); // (2, 0)
            var y2 = term.Differentiate(v, Vec(0.0, 1.0)); // (0, -2)
            var y3 = term.Differentiate(v, Vec(2.0, 1.0)); // (4, -2)

            Assert.Equal(Vec(2.0, 0.0), y1);
            Assert.Equal(Vec(0.0, -2.0), y2);
            Assert.Equal(Vec(4.0, -2.0), y3);
        }

        [Fact]
        public void DiffParametric()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();
            var m = new Variable();
            var n = new Variable();

            var func = x + 2 * y + 3 * z + 4 * m + 5 * n;
            var compiled = func.Compile(Vec(x, y, z), Vec(m, n));

            var diffResult = compiled.Differentiate(NumVec(1, 1, 1), NumVec(1, 1));
            Assert.Equal(15, diffResult.Item2); // 15 = 1 + 2 + 3 + 4 + 5
            Assert.Equal(NumVec(1, 2, 3), diffResult.Item1);
        }

        [Fact]
        public void CompilesUsingAnyList()
        {
            var x = new Variable();
            var y = new Variable();

            IReadOnlyList<Variable> variables = new List<Variable> { x, y };
            var func = x + y;

            func.Compile(variables);
        }

        [Fact]
        public void DifferentiatesUsingAnyList()
        {
            var x = new Variable();
            var y = new Variable();

            var func = 0.5 * (x*x + y*y); // we chose this function so that the gradient at (x, y) will be just (x, y)
            var compiled = func.Compile(x, y);

            IReadOnlyList<double> input = new List<double> { 2, 6 };
            var diff = compiled.Differentiate(input);

            Assert.Equal(diff.Item1, Vec(2.0, 6.0));
            Assert.Equal(20, diff.Item2); // 20 = 0.5 * (2^2 + 6^2)
        }

        [Fact]
        public void FillsGradientList()
        {
            var x = new Variable();
            var y = new Variable();

            var func = 0.5*(x*x + y*y); 
            var compiled = func.Compile(x, y);

            var input = new[] {2.0, 6.0 };
            IList<double> grad = new double[input.Length];
            var value = compiled.Differentiate(input, grad);

            Assert.Equal(grad, input); // the gradient at (x, y) is (x, y)
            Assert.Equal(20, value); // 20 = 0.5 * (2^2 + 6^2)
        }
    }
}
