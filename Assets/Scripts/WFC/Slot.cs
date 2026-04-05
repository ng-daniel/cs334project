namespace WFC
{
    public class Slot
    {
        public readonly int x;
        public readonly int y;

        // Whether each module is in this slot's superposition
        public readonly bool[] wave;

        public int possibleModuleCount;
        public Module module;

        public Slot(int x, int y)
        {
            this.x = x;
            this.y = y;

            wave = new bool[WFC.instance.ModuleCount()];
            for (int m = 0; m < wave.Length; m++)
            {
                wave[m] = true;
            }

            possibleModuleCount = wave.Length;
            module = null;
        }

        public void Remove(int m)
        {
            wave[m] = false;
            possibleModuleCount--;
        }
    }
}
