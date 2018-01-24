using System;
using Xunit;

namespace AutoDiff.Tests
{
    public class TermOperatorContractTests
    {
        Term x = new Variable();

        [Fact]
        public void PlusContract()
        {
            Assert.Throws<ArgumentNullException>(() => x + null);
            Assert.Throws<ArgumentNullException>(() => null + x);
        }

        [Fact]
        public void StarContract()
        {
            Assert.Throws<ArgumentNullException>(() => x * null);
            Assert.Throws<ArgumentNullException>(() => null * x);
        }

        [Fact]
        public void UnaryMinusContract()
        {
            Term trm = null;
            Assert.Throws<ArgumentNullException>(() => -trm);
        }

        [Fact]
        public void BinaryMinusContract()
        {
            Assert.Throws<ArgumentNullException>(() => x - null);
            Assert.Throws<ArgumentNullException>(() => null - x);
        }

        [Fact]
        public void SlashContract()
        {
            Assert.Throws<ArgumentNullException>(() => null / x);
            Assert.Throws<ArgumentNullException>(() => x / null);
        }
    }
}
