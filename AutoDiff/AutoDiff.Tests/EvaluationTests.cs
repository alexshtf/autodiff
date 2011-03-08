using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoDiff.Tests
{
    [TestClass]
    public class EvaluationTests
    {
        private static readonly Variable[] NoVars = new Variable[0];
        private static readonly double[] NoVals = new double[0];

        [TestMethod]
        public void TestZero()
        {
            var zero = TermBuilder.Constant(0);
            var value = Evaluator.Evaluate(zero, NoVars, NoVals);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void TestConstant()
        {
            var constant = TermBuilder.Constant(5);
            var value = Evaluator.Evaluate(constant, NoVars, NoVals);
            Assert.AreEqual(5, value);
        }

        [TestMethod]
        public void TestSumTwoConsts()
        {
            var c1 = TermBuilder.Constant(5);
            var c2 = TermBuilder.Constant(7);
            var sum = c1 + c2;
            var value = Evaluator.Evaluate(sum, NoVars, NoVals);
            Assert.AreEqual(12, value);
        }

        [TestMethod]
        public void TestSumConstVar()
        {
            var c = TermBuilder.Constant(5);
            var v = new Variable();
            var sum = c + v;
            var value = Evaluator.Evaluate(sum, Utils.Array(v), Utils.Array(7.0));
            Assert.AreEqual(12, value);
        }

        [TestMethod]
        public void TestDiffConst()
        {
            var c1 = TermBuilder.Constant(12);
            var c2 = TermBuilder.Constant(5);
            var diff = c1 - c2;
            var value = Evaluator.Evaluate(diff, NoVars, NoVals);
            Assert.AreEqual(7, value);
        }

        [TestMethod]
        public void TestDiffVar()
        {
            var c = TermBuilder.Constant(12);
            var v = new Variable();
            var diff = c - v;
            var value = Evaluator.Evaluate(diff, Utils.Array(v), Utils.Array(5.0));
            Assert.AreEqual(7, value);
        }

        [TestMethod]
        public void TestProdVar()
        {
            var v1 = new Variable();
            var v2 = new Variable();
            var prod = v1 * v2;
            var value = Evaluator.Evaluate(prod, Utils.Array(v1, v2), Utils.Array(3.0, -5));
            Assert.AreEqual(-15, value);
        }

        [TestMethod]
        public void TestConstPower()
        {
            var c = TermBuilder.Constant(3);
            var pow = TermBuilder.Power(c, 3);
            var value = Evaluator.Evaluate(pow, NoVars, NoVals);
            Assert.AreEqual(27, value);
        }

        [TestMethod]
        public void TestSquareDiff()
        {
            var v = new Variable();
            var sqDiff = TermBuilder.Power(v - 5, 2);
            
            var r1 = Evaluator.Evaluate(sqDiff, Utils.Array(v), Utils.Array(3.0));
            var r2 = Evaluator.Evaluate(sqDiff, Utils.Array(v), Utils.Array(5.0));

            Assert.AreEqual(4, r1);
            Assert.AreEqual(0, r2);
        }

        [TestMethod]
        public void WeighedSquareDiff()
        {
            var v = Utils.Array(new Variable(), new Variable(), new Variable());
            var sqDiff = TermBuilder.Sum(
                12 * TermBuilder.Power(v[0] - 5, 2),
                3 * TermBuilder.Power(v[1] - 4, 2),
                2 * TermBuilder.Power(v[2] + 3, 2));

            var r1 = Evaluator.Evaluate(sqDiff, v, Utils.Array(5.0, 4.0, -3.0)); 
            var r2 = Evaluator.Evaluate(sqDiff, v, Utils.Array(3.0, 4.0, -3.0));
            var r3 = Evaluator.Evaluate(sqDiff, v, Utils.Array(4.0, 4.0, 0.0));

            Assert.AreEqual(r1, 0);
            Assert.AreEqual(r2, 48);
            Assert.AreEqual(r3, 30);
        }
    }
}
