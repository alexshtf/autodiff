using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    public class TermBuilder
    {
        public static Term Constant(double value)
        {
            if (value == 0)
                return new Zero();
            else
                return new Constant(value);
        }

        public static Sum Sum(IEnumerable<Term> terms)
        {
            Contract.Requires(terms.Count() > 1);
            terms = terms.Where(term => !(term is Zero));
            return new Sum(terms);
        }

        public static Sum Sum(Term v1, Term v2, params Term[] rest)
        {
            var allTerms = new Term[] { v1, v2 }.Concat(rest);
            return Sum(allTerms);
        }

        public static Term Product(Term v1, Term v2, params Term[] rest)
        {
            var result = new Product(v1, v2);
            foreach (var item in rest)
                result = new Product(result, item);

            return result;
        }

        public static Term Power(Term t, double power)
        {
            return new IntPower(t, power);
        }

        public static Term Exp(Term arg)
        {
            return new Exp(arg);
        }

        public static Term Log(Term arg)
        {
            return new Log(arg);
        }

        public static Term Piecewise(IEnumerable<Tuple<Inequality, Term>> pieces)
        {
            return new PiecewiseTerm(pieces);
        }

        public static Term Piecewise(params Tuple<Inequality, Term>[] pieces)
        {
            return Piecewise(pieces as IEnumerable<Tuple<Inequality, Term>>);
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
            return Sum(a11 * Power(x1, 2), (a12 + a21) * x1 * x2, a22 * Power(x2, 2));
        }
    }
}
