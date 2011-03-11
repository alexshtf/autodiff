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
            var grad = Differentiator.Differentiate(zero, Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(0), grad);
        }

        [TestMethod]
        public void DiffConstant()
        {
            var c = TermBuilder.Constant(100);
            var v = new Variable();
            var grad = Differentiator.Differentiate(c, Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(0), grad);
        }

        [TestMethod]
        public void DiffVar()
        {
            var v = new Variable();
            var grad = Differentiator.Differentiate(v, Utils.Array(v), Utils.Vector(12));
            CollectionAssert.AreEqual(Utils.Vector(1), grad);
        }

        [TestMethod]
        public void DiffProdTwoVars()
        {
            var x = new Variable();
            var y = new Variable();
            var func = x * y;
            var grad = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(3, -4));
            CollectionAssert.AreEqual(Utils.Vector(-4, 3), grad);
        }

        [TestMethod]
        public void DiffProdVarsConst()
        {
            var x = new Variable();
            var y = new Variable();
            var func = -3 * x * y;
            var grad = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(2, -3));
            CollectionAssert.AreEqual(Utils.Vector(9, -6), grad);
        }

        [TestMethod]
        public void DiffQuadraticSingleVar()
        {
            var x = new Variable();

            // f(x) = 2x²
            var func = 2 * TermBuilder.Power(x, 2);
            var grad = Differentiator.Differentiate(func, Utils.Array(x), Utils.Vector(2));

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
            var grad = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(2, 3));
            
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
            var gradOpt = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(5, -2));
            var gradGen = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(3, 1));

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
            var grad1 = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(1, 4));
            grad1[0] = Math.Round(grad1[0], 2);
            grad1[1] = Math.Round(grad1[1], 2);
            CollectionAssert.AreEqual(Utils.Vector(-0.92, 0.88), grad1);

            // df(-6,4) = [-11, -26]
            var grad2 = Differentiator.Differentiate(func, Utils.Array(x, y), Utils.Vector(-6, 4));
            CollectionAssert.AreEqual(Utils.Vector(-11, -26), grad2);
        }

        [TestMethod]
        public void DiffExp()
        {
            var x = new Variable();
            var func = TermBuilder.Exp(x);

            var grad1 = Differentiator.Differentiate(func, Utils.Array(x), Utils.Vector(1));
            var grad2 = Differentiator.Differentiate(func, Utils.Array(x), Utils.Vector(-2));

            CollectionAssert.AreEqual(Utils.Vector(Math.Exp(1)), grad1);
            CollectionAssert.AreEqual(Utils.Vector(Math.Exp(-2)), grad2);
        }
    }
}
