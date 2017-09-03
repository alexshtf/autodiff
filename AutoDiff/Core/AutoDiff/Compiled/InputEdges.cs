using System.Runtime.CompilerServices;

namespace AutoDiff.Compiled
{
    internal struct InputEdges
    {
        private readonly InputEdge[] array;
        private readonly int offset;
        private readonly int length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InputEdges(InputEdge[] array)
            : this(array, 0, array.Length)
        {}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InputEdges(InputEdge[] array, int offset, int length)
        {
            this.array = array;
            this.offset = offset;
            this.length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InputEdges(int offset, int length)
            : this(null, offset, length)
        {}

        public InputEdges Remap(InputEdge[] newArray) => new InputEdges(newArray, this.offset, this.length);

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return length; }
        }
       
        public int Index(int i) => array[offset + i].Index;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Weight(int i) => array[offset + i].Weight;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWeight(int i, double w) => array[offset + i].Weight = w;
    }
}