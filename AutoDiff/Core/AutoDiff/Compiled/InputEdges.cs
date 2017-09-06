using System.Runtime.CompilerServices;

namespace AutoDiff.Compiled
{
    internal struct InputEdges
    {
        private readonly InputEdge[] array;
        private readonly int offset;
        private readonly int length;

        private InputEdges(InputEdge[] array, int offset, int length)
        {
            this.array = array;
            this.offset = offset;
            this.length = length;
        }

        public InputEdges(int offset, int length)
            : this(null, offset, length)
        {}

        public InputEdges Remap(InputEdge[] newArray) => new InputEdges(newArray, this.offset, this.length);
        public int Length => length;
        public int Index(int i) => array[offset + i].Index;
        public double Weight(int i) => array[offset + i].Weight;
        public void SetWeight(int i, double w) => array[offset + i].Weight = w;
    }
}