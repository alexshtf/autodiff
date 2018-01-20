using System.Collections.Generic;
using System.Linq;

namespace AutoDiff
{
    /// <summary>
    /// A collection of static methods to build new terms
    /// </summary>
    public static class TermBuilder
    {
        /// <summary>
        /// Builds a new constant term.
        /// </summary>
        /// <param name="value">The constant value</param>
        /// <returns>The constant term.</returns>
        public static Term Constant(double value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == 0)
                return new Zero();
            else
                return new Constant(value);
        }

        /// <summary>
        /// Builds a sum of given terms.
        /// </summary>
        /// <param name="terms">The collection of terms in the sum.</param>
        /// <returns>A term representing the sum of the terms in <paramref name="terms"/>.</returns>
        public static Sum Sum(IEnumerable<Term> terms)
        {
            Guard.CollectionAndItemsNotNull(terms, nameof(terms));

            terms = terms.Where(term => !(term is Zero));
            return new Sum(terms);
        }

        /// <summary>
        /// Builds a sum of given terms.
        /// </summary>
        /// <param name="v1">The first term in the sum</param>
        /// <param name="v2">The second term in the sum</param>
        /// <param name="rest">The rest of the terms in the sum.</param>
        /// <returns>A term representing the sum of <paramref name="v1"/>, <paramref name="v2"/> and the terms in <paramref name="rest"/>.</returns>
        public static Sum Sum(Term v1, Term v2, params Term[] rest)
        {
            Guard.NotNull(v1, nameof(v1));
            Guard.NotNull(v2, nameof(v2));
            Guard.ItemsNotNull(rest, nameof(rest));

            var allTerms = new[] { v1, v2 }.Concat(rest);
            return Sum(allTerms);
        }

        /// <summary>
        /// Builds a product of given terms.
        /// </summary>
        /// <param name="v1">The first term in the product</param>
        /// <param name="v2">The second term in the product</param>
        /// <param name="rest">The rest of the terms in the product</param>
        /// <returns>A term representing the product of <paramref name="v1"/>, <paramref name="v2"/> and the terms in <paramref name="rest"/>.</returns>
        public static Term Product(Term v1, Term v2, params Term[] rest)
        {
            Guard.NotNull(v1, nameof(v1));
            Guard.NotNull(v2, nameof(v2));
            Guard.CollectionAndItemsNotNull(rest, nameof(rest));

            return rest.Aggregate(new Product(v1, v2), (product, item) => new Product(product, item));
        }

        /// <summary>
        /// Builds a power terms given a base and a constant exponent
        /// </summary>
        /// <param name="t">The power base term</param>
        /// <param name="power">The exponent</param>
        /// <returns>A term representing <c>t^power</c>.</returns>
        public static Term Power(Term t, double power)
        {
            Guard.NotNull(t, nameof(t));
            Guard.MustHold(!double.IsNaN(power) && !double.IsInfinity(power) && power != 0, "power must be finite and non-zero");

            return new ConstPower(t, power);
        }

        /// <summary>
        /// Builds a power term given a base term and an exponent term.
        /// </summary>
        /// <param name="baseTerm">The base term</param>
        /// <param name="exponent">The exponent term</param>
        /// <returns></returns>
        public static Term Power(Term baseTerm, Term exponent)
        {
            Guard.NotNull(baseTerm, nameof(baseTerm));
            Guard.NotNull(exponent, nameof(exponent));

            return new TermPower(baseTerm, exponent);
        }

        /// <summary>
        /// Builds a term representing the exponential function e^x.
        /// </summary>
        /// <param name="arg">The function's exponent</param>
        /// <returns>A term representing e^arg.</returns>
        public static Term Exp(Term arg)
        {
            Guard.NotNull(arg, nameof(arg));

            return new Exp(arg);
        }

        /// <summary>
        /// Builds a term representing the natural logarithm.
        /// </summary>
        /// <param name="arg">The natural logarithm's argument.</param>
        /// <returns>A term representing the natural logarithm of <paramref name="arg"/></returns>
        public static Term Log(Term arg)
        {
            Guard.NotNull(arg, nameof(arg));

            return new Log(arg);
        }

        /// <summary>
        /// Constructs a 2D quadratic form given the vector components x1, x2 and the matrix coefficients a11, a12, a21, a22.
        /// </summary>
        /// <param name="x1">First vector component</param>
        /// <param name="x2">Second vector component</param>
        /// <param name="a11">First row, first column matrix component</param>
        /// <param name="a12">First row, second column matrix component</param>
        /// <param name="a21">Second row, first column matrix component</param>
        /// <param name="a22">Second row, second column matrix component</param>
        /// <returns>A term describing the quadratic form</returns>
        public static Term QuadForm(Term x1, Term x2, Term a11, Term a12, Term a21, Term a22)
        {
            Guard.NotNull(x1, nameof(x1));
            Guard.NotNull(x2, nameof(x2));
            Guard.NotNull(a11, nameof(a11));
            Guard.NotNull(a12, nameof(a12));
            Guard.NotNull(a21, nameof(a21));
            Guard.NotNull(a22, nameof(a22));

            return Sum(a11 * Power(x1, 2), (a12 + a21) * x1 * x2, a22 * Power(x2, 2));
        }
    }
}
