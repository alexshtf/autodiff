using static AutoDiff.Tests.Utils;
using Xunit;
using System;
using System.Collections.Generic;

namespace AutoDiff.Tests
{
    public class TermBuilderContractTests
    {
        private static readonly Variable x = new Variable();
        private static readonly Variable y = new Variable();

        [Fact]
        public void ConstantContract()
        {
            Assert.IsType<Constant>(TermBuilder.Constant(1));
            Assert.IsType<Zero>(TermBuilder.Constant(0));
        }

        [Fact]
        public void SumValidationContract()
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(null));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(x, null));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(null, x));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(x, y, null));
            Assert.Throws<ArgumentException>(() => TermBuilder.Sum(x, y, Vec(x, null)));
            Assert.Throws<ArgumentException>(() => TermBuilder.Sum(Vec(x, null)));
        }

        [Fact]
        public void ProductContract()
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(x, null));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(null, x));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(x, y, null));
            Assert.Throws<ArgumentException>(() => TermBuilder.Product(x, y, Vec(x, null)));
            Assert.IsType<Product>(TermBuilder.Product(x, y));
        }

        [Fact]
        public void PowerContract()
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(null, 1));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(x, null));
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(null, x));
            Assert.IsType<ConstPower>(TermBuilder.Power(x, 1));

            Assert.Throws<ArgumentException>(() => TermBuilder.Power(x, double.NaN));
            Assert.Throws<ArgumentException>(() => TermBuilder.Power(x, double.PositiveInfinity));
            Assert.IsType<TermPower>(TermBuilder.Power(x, y));
            Assert.IsType<TermPower>(TermBuilder.Power(1, y));
        }

        [Fact]
        public void ExpContract()
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Exp(null));
            Assert.IsType<Exp>(TermBuilder.Exp(x));
        }

        [Fact]
        public void LogContract()
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.Log(null));
            Assert.IsType<Log>(TermBuilder.Log(x));
        }

        [Theory]
        [MemberData(nameof(QuadFormContractData))]
        public void QuadFormContract(Term x, Term y, Term a11, Term a21, Term a12, Term a22)
        {
            Assert.Throws<ArgumentNullException>(() => TermBuilder.QuadForm(x, y, a11, a21, a12, a22));
        }

        public static IEnumerable<object[]> QuadFormContractData() =>
            new[]
            {
                new Term[] { null, y, 1, 2, 3, 4 },
                new Term[] { x, null, 1, 2, 3, 4 },
                new Term[] { x, y, null, 2, 3, 4 },
                new Term[] { x, y, 1, null, 3, 4 },
                new Term[] { x, y, 1, 2, null, 4 },
                new Term[] { x, y, 1, 2, 3, null },
            };
    }
}
