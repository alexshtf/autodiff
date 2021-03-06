namespace AutoDiff.Compiled
{
    internal struct InputEdges
    {
        private readonly InputEdge[] edges;
        private readonly int offset;

        private InputEdges(InputEdge[] edges, int offset, int length)
        {
            this.edges = edges;
            this.offset = offset;
            Length = length;
        }

        public InputEdges(int offset, int length)
            : this(null, offset, length)
        {}

        public InputEdges Remap(InputEdge[] newEdges) => new InputEdges(newEdges, this.offset, this.Length);
        public int Length { get; }
        public TapeElement Element(int i) => edges[offset + i].Element;
        public double Weight(int i) => edges[offset + i].Weight;
        public void SetWeight(int i, double w) => edges[offset + i].Weight = w;
    }
}