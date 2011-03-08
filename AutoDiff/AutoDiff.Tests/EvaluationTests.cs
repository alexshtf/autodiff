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
        [TestMethod]
        public void TestZero()
        {
            var zero = TermBuilder.Constant(0);
            var value = Evaluator.Evaluate(zero, new Variable[0], new double[0]);
            Assert.AreEqual(0, value);
        }
    }
}
