namespace WFC
{
    public class Slot
    {
        public readonly Chunk chunk;
        public readonly int x;
        public readonly int y;

        // Whether each module is in this slot's superposition
        public readonly bool[] wave;

        // For each module, for each direction, the number of modules
        // that could be in that direction for that module
        // If any direction is 0, the module cannot go in this slot
        public readonly int[][] compatibility;

        public int possibleModuleCount;
        public Module module;

        public Slot(Chunk chunk, int x, int y)
        {
            this.chunk = chunk;
            this.x = x;
            this.y = y;

            wave = new bool[WFC.instance.ModuleCount()];
            compatibility = new int[WFC.instance.ModuleCount()][];

            for (int m = 0; m < WFC.instance.ModuleCount(); m++)
            {
                wave[m] = true;
                compatibility[m] = new int[Direction.COUNT];

                for (int d = 0; d < Direction.COUNT; d++)
                {
                    compatibility[m][d] = WFC.instance.adjacencies[m][d].Count;
                }
            }

            possibleModuleCount = wave.Length;
            module = null;
        }

        public void Remove(int m)
        {
            wave[m] = false;
            possibleModuleCount--;

            for (int d = 0; d < Direction.COUNT; d++)
            {
                compatibility[m][d] = 0;
            }
        }

        public Slot Neighbor(int d)
        {
            return chunk.GetSlot(x + Direction.OffsetX(d), y + Direction.OffsetY(d));
        }
    }
}
