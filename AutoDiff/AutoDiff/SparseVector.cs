using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    class SparseVector
    {
        private Tuple<int, double>[] nonZeros;
        private int count;

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(nonZeros != null);
            Contract.Invariant(nonZeros.Length >= count);
        }

        public SparseVector()
        {
            nonZeros = new Tuple<int, double>[0];
        }

        public SparseVector(int index, double value)
            : this(1)
        {
            nonZeros[0] = Tuple.Create(index, value);
            count = 1;
        }

        private SparseVector(int capacity)
        {
            nonZeros = new Tuple<int, double>[capacity];
        }

        [ContractVerification(true)]
        private void AddTuple(Tuple<int, double> tuple)
        {
            if (nonZeros.Length == count)
            {
                var newNonZeros = new Tuple<int, double>[2 * nonZeros.Length + 1];
                Array.Copy(nonZeros, 0, newNonZeros, 0, count);
                nonZeros = newNonZeros;
            }

            UnsafeAdd(tuple);
        }

        private void UnsafeAdd(Tuple<int, double> tuple)
        {
            nonZeros[count] = tuple;
            ++count;
        }

        public double[] ToArray(int length)
        {
            Contract.Assert(count == 0 || length >= nonZeros[count - 1].Item1 + 1); // length is greater than the last non-zero index

            double[] result = new double[length];
            for (int i = 0; i < count; ++i)
            {
                var item = nonZeros[i];
                var idx = item.Item1;
                var val = item.Item2;
                result[idx] = val;
            }

            return result;
        }

        public static SparseVector Scale(SparseVector v, double scale)
        {
            if (scale == 0)
                return new SparseVector();
            else
            {
                var result = new SparseVector(v.nonZeros.Length);
                for (int i = 0; i < v.count; i++)
                {
                    var item = v.nonZeros[i];
                    result.UnsafeAdd(Tuple.Create(item.Item1, scale * item.Item2));
                }

                return result;
            }
        }

        public static SparseVector Sum(SparseVector left, SparseVector right)
        {
            var result = new SparseVector(left.count + right.count);

            int i = 0;
            int j = 0;
            while (i < left.count && j < right.count)
            {
                var leftIdx = left.nonZeros[i].Item1;
                var rightIdx = right.nonZeros[j].Item1;

                if (leftIdx < rightIdx)
                {
                    result.UnsafeAdd(left.nonZeros[i]);
                    ++i;                
                }
                else if (rightIdx < leftIdx)
                {
                    result.UnsafeAdd(right.nonZeros[j]);
                    ++j;
                }
                else
                {
                    var leftVal = left.nonZeros[i].Item2;
                    var rightVal = right.nonZeros[j].Item2;
                    result.UnsafeAdd(Tuple.Create(leftIdx, leftVal + rightVal));
                    ++i;
                    ++j;
                }
            }

            while (i < left.count)
            {
                result.UnsafeAdd(left.nonZeros[i]);
                ++i;
            }

            while (j < right.count)
            {
                result.UnsafeAdd(right.nonZeros[j]);
                ++j;
            }

            return result;
        }
    }
}
