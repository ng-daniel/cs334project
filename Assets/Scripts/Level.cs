namespace WFC
{
    public class Level
    {
        public Slot[] slots;
        public int chunkSize = 0;

        public Level(Chunk chunk, int chunkSize)
        {
            this.chunkSize = chunkSize;

            this.slots = new Slot[chunkSize * chunkSize];

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Slot(chunk, GetX(i), GetY(i));
            }
        }
        public Slot GetSlot(int x, int y)
        {
            if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize)
            {
                return null;
            }
            return slots[y * chunkSize + x];
        }

        public int GetX(int slotIndex)
        {
            return slotIndex % chunkSize;
        }

        public int GetY(int slotIndex)
        {
            return slotIndex / chunkSize;
        }
    }
}