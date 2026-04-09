namespace WFC
{
    public class Chunk
    {
        public const int CHUNK_SIZE = 30;

        public int chunkX;
        public int chunkY;
        public Slot[] slots;

        public Chunk(int chunkX, int chunkY)
        {
            this.chunkX = chunkX;
            this.chunkY = chunkY;
            slots = new Slot[CHUNK_SIZE * CHUNK_SIZE];

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Slot(this, GetX(i), GetY(i));
            }
        }

        public Slot GetSlot(int x, int y)
        {
            if (x < 0 || x >= CHUNK_SIZE || y < 0 || y >= CHUNK_SIZE)
            {
                return null;
            }
            return slots[y * CHUNK_SIZE + x];
        }

        public int GetX(int slotIndex)
        {
            return slotIndex % CHUNK_SIZE;
        }

        public int GetY(int slotIndex)
        {
            return slotIndex / CHUNK_SIZE;
        }

        public void PostGeneration()
        {
            foreach (Slot slot in slots)
            {
                slot.Spawn();
            }
        }
    }
}