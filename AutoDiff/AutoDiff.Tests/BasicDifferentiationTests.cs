using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoDiff.Tests
{
    [TestClass]
    public class BasicDifferentiationTests
    {
        [TestMethod]
        public void DiffZero()
        {
            var zero = TermBuilder.Constant(0);
            var v = new Variable();
            var grad = zero.Differentiate(Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(0), grad);
        }

        [TestMethod]
        public void DiffConstant()
        {
            var c = TermBuilder.Constant(100);
            var v = new Variable();
            var grad = c.Differentiate(Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(0), grad);
        }

        [TestMethod]
        public void DiffVar()
        {
            var v = new Variable();
            var grad = v.Differentiate(Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(1), grad);
        }

        [TestMethod]
        public void DiffPartialVars()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();
            var grad = y.Differentiate(Utils.Array(x, y, z), Utils.Vector(1, 2, 3));
            CollectionAssert.AreEqual(Utils.Vector(0, 1, 0), grad);
        }

        [TestMethod]
        public void DiffProdTwoVars()
        {
            var x = new Variable();
            var y = new Variable();
            var func = x * y;
            var grad = func.Differentiate(Utils.Array(x, y), Utils.Vector(3, -4));
            CollectionAssert.AreEqual(Utils.Vector(-4, 3), grad);
        }

        [TestMethod]
        public void DiffProdVarsConst()
        {
            var x = new Variable();
            var y = new Variable();
            var func = -3 * x * y;
            var grad = func.Differentiate(Utils.Array(x, y), Utils.Vector(2, -3));
            CollectionAssert.AreEqual(Utils.Vector(9, -6), grad);
        }

        [TestMethod]
        public void DiffQuadraticSingleVar()
        {
            var x = new Variable();

            // f(x) = 2x²
            var func = 2 * TermBuilder.Power(x, 2);
            var grad = func.Differentiate(Utils.Array(x), Utils.Vector(2));

            // df(x) = 4x
            CollectionAssert.AreEqual(Utils.Vector(8), grad);
        }

        [TestMethod]
        public void DiffQuadraticTwoVars()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = 2x² - 3y²
            var func = 2 * TermBuilder.Power(x, 2) - 3 * TermBuilder.Power(y, 2);
            var grad = func.Differentiate(Utils.Array(x, y), Utils.Vector(2, 3));
            
            // df(x, y) = (4x, -6y)
            CollectionAssert.AreEqual(Utils.Vector(8, -18), grad);
        }

        [TestMethod]
        public void DiffMeanSquaredError()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = (x - 5)² + (x + 2)²
            var func = 0.5 * (TermBuilder.Power(x - 5, 2) + TermBuilder.Power(y + 2, 2));
            var gradOpt = func.Differentiate(Utils.Array(x, y), Utils.Vector(5, -2));
            var gradGen = func.Differentiate(Utils.Array(x, y), Utils.Vector(3, 1));

            // df(x, y) = [x-5, x+2]
            CollectionAssert.AreEqual(Utils.Vector(0, 0), gradOpt);
            CollectionAssert.AreEqual(Utils.Vector(-2, 3), gradGen);
        }

        [TestMethod]
        public void DiffRational()
        {
            var x = new Variable();
            var y = new Variable();

            // f(x, y) = (x² - xy + y²) / (x + y)
            var func = 
                (TermBuilder.Power(x, 2) - x * y + TermBuilder.Power(y, 2)) * 
                TermBuilder.Power(x + y, -1);

            // df(1,4) = [-0.92, 0.88]
            var grad1 = func.Differentiate(Utils.Array(x, y), Utils.Vector(1, 4));
            grad1[0] = Math.Round(grad1[0], 2);
            grad1[1] = Math.Round(grad1[1], 2);
            CollectionAssert.AreEqual(Utils.Vector(-0.92, 0.88), grad1);

            // df(-6,4) = [-11, -26]
            var grad2 = func.Differentiate(Utils.Array(x, y), Utils.Vector(-6, 4));
            CollectionAssert.AreEqual(Utils.Vector(-11, -26), grad2);
        }

        [TestMethod]
        public void DiffPolynomial1()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();

            // f(x,y,z) = 2(x-y)² + 5xy - 3y²
            var func = 2 * TermBuilder.Power(x - y, 2) + 5 * x * y - 3 * TermBuilder.Power(y, 2);
            var diff = func.Compile(x, y, z);

            var result = diff.Differentiate(Utils.Vector(1, 2, -3));
            CollectionAssert.AreEqual(Utils.Vector(6, -3, 0), result.Item1);
            Assert.AreEqual(0, result.Item2);
        }

        [TestMethod]
        public void DiffPolynomial2()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();

            var terms = new Term[] 
                { 
                    x + 2 * TermBuilder.Power(x - y, 2), // x + 2(x-y)²
                    x*y - y*z,                           // xy - yz
                    3 * x * y * z,                       // 3xyz
                };

            // (x + 2(x-y)²)², (xy - yz)², (3xyz)²
            terms = terms.Select(t => TermBuilder.Power(t, 2)).ToArray();

            // 0.25 * ((x + 2(x-y)²)² + (xy - yz)² + (3xyz)²) + (y - x + 1)²
            var func = 0.25 * TermBuilder.Sum(terms) + TermBuilder.Power(y - x + 1, 2);

            var diff = func.Compile(x, y, z);
            var result = diff.Differentiate(1, 2, -3);

            // asserts checked with MATLAB
            CollectionAssert.AreEqual(Utils.Vector(161.5, 107, -62), result.Item1);
            Assert.AreEqual(103.25, result.Item2);
        }

        [TestMethod]
        public void DiffExp()
        {
            var x = new Variable();
            var func = TermBuilder.Exp(x);

            var grad1 = func.Differentiate(Utils.Array(x), Utils.Vector(1));
            var grad2 = func.Differentiate(Utils.Array(x), Utils.Vector(-2));

            CollectionAssert.AreEqual(Utils.Vector(Math.Exp(1)), grad1);
            CollectionAssert.AreEqual(Utils.Vector(Math.Exp(-2)), grad2);
        }

        [TestMethod]
        public void DiffTermPower()
        {
            var x = new Variable();
            var y = new Variable();
            var func = TermBuilder.Power(x, y);

            var grad = func.Differentiate(Utils.Array(x, y), Utils.Vector(2, 3));

            CollectionAssert.AreEqual(Utils.Vector(12, 8 * Math.Log(2)), grad);
        }

        [TestMethod]
        public void DiffTermPowerSingleVariable()
        {
            var x = new Variable();
            var func = TermBuilder.Power(x, x);

            var grad = func.Differentiate(Utils.Array(x), Utils.Vector(2.5));
            var expectedGrad = Utils.Vector(Math.Pow(2.5, 2.5) * (Math.Log(2.5) + 1));
            Assert.IsTrue(Math.Abs(grad[0] - expectedGrad[0]) < 1E-10);
        }

        [TestMethod]
        public void DiffUnarySimple()
        {
            var v = new Variable();

            Func<double, double> eval = x => x * x;
            Func<double, double> diff = x => 2 * x;

            var term = new UnaryFunc(eval, diff, v);

            var y1 = term.Differentiate(Utils.Array(v), Utils.Array(1.0)); // 2
            var y2 = term.Differentiate(Utils.Array(v), Utils.Array(2.0)); // 4
            var y3 = term.Differentiate(Utils.Array(v), Utils.Array(3.0)); // 6

            CollectionAssert.AreEqual(Utils.Array(2.0), y1);
            CollectionAssert.AreEqual(Utils.Array(4.0), y2);
            CollectionAssert.AreEqual(Utils.Array(6.0), y3);
        }

        [TestMethod]
        public void DiffUnaryComplex()
        {
            var square = UnaryFunc.Factory(x => x * x, x => 2 * x);

            // f(x, y) = x^2 + 2 * y^2
            // df/dx = 2 * x
            // df/dy = 4 * y
            var v = Utils.Array(new Variable(), new Variable());
            var term = square(v[0]) + 2 * square(v[1]);

            var y1 = term.Differentiate(v, Utils.Array(1.0, 0.0));  // (2.0, 0.0)
            var y2 = term.Differentiate(v, Utils.Array(0.0, 1.0));  // (0.0, 4.0)
            var y3 = term.Differentiate(v, Utils.Array(2.0, 1.0));  // (4.0, 4.0)

            CollectionAssert.AreEqual(Utils.Array(2.0, 0.0), y1);
            CollectionAssert.AreEqual(Utils.Array(0.0, 4.0), y2);
            CollectionAssert.AreEqual(Utils.Array(4.0, 4.0), y3);
        }

        [TestMethod]
        public void DiffBinarySimple()
        {
            var v = Utils.Array(new Variable(), new Variable());
            var func = BinaryFunc.Factory(
                (x, y) => x * x - x * y,
                (x, y) => Tuple.Create(2 * x - y, -x));

            // f(x, y) = x² - xy
            // df/dx = 2x - y
            // df/dy = -x
            var term = func(v[0], v[1]);

            var y1 = term.Differentiate(v, Utils.Array(1.0, 0.0)); // (2, -1)
            var y2 = term.Differentiate(v, Utils.Array(0.0, 1.0)); // (-1, 0)
            var y3 = term.Differentiate(v, Utils.Array(1.0, 2.0)); // (0, -1)

            CollectionAssert.AreEqual(Utils.Array(2.0, -1.0), y1);
            CollectionAssert.AreEqual(Utils.Array(-1.0, 0.0), y2);
            CollectionAssert.AreEqual(Utils.Array(0.0, -1.0), y3);
        }

        [TestMethod]
        public void DiffBinaryComplex()
        {
            var v = Utils.Array(new Variable(), new Variable());
            var func = BinaryFunc.Factory(
                (x, y) => x * x - x * y,
                (x, y) => Tuple.Create(2 * x - y, -x));

            // f(x, y) = x² - xy - y² + xy = x² - y²
            // df/dx = 2x
            // df/dy = -2y
            var term = func(v[0], v[1]) - func(v[1], v[0]);

            var y1 = term.Differentiate(v, Utils.Array(1.0, 0.0)); // (2, 0)
            var y2 = term.Differentiate(v, Utils.Array(0.0, 1.0)); // (0, -2)
            var y3 = term.Differentiate(v, Utils.Array(2.0, 1.0)); // (4, -2)

            CollectionAssert.AreEqual(Utils.Array(2.0, 0.0), y1);
            CollectionAssert.AreEqual(Utils.Array(0.0, -2.0), y2);
            CollectionAssert.AreEqual(Utils.Array(4.0, -2.0), y3);
        }

        [TestMethod]
        public void DiffParametric()
        {
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();
            var m = new Variable();
            var n = new Variable();

            var func = x + 2 * y + 3 * z + 4 * m + 5 * n;
            var compiled = func.Compile(Utils.Array(x, y, z), Utils.Array(m, n));

            var diffResult = compiled.Differentiate(Utils.Vector(1, 1, 1), Utils.Vector(1, 1));
            Assert.AreEqual(15, diffResult.Item2); // 15 = 1 + 2 + 3 + 4 + 5
            CollectionAssert.AreEqual(Utils.Vector(1, 2, 3), diffResult.Item1);
        }
    }
}
