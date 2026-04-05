namespace WFC
{
    public class Module
    {
        // Whether each of the four edges is solid
        public readonly bool[] edges;

        // Symmetry?

        //public GameObject prefab;

        public Module(bool north, bool east, bool south, bool west)
        {
            edges = new bool[4];
            edges[0] = north;
            edges[1] = east;
            edges[2] = south;
            edges[3] = west;
        }
    }
}