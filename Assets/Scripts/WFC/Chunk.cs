using System.Collections;
using System.Collections.Generic;

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

        public IEnumerable PostGeneration()
        {
            PruneUnreachablePaths();

            foreach (Slot slot in slots)
            {
                slot.Spawn();
                yield return null;
            }
        }

        // Delete all paths that aren't reachable from the edge of a chunk
        private void PruneUnreachablePaths()
        {
            bool[] reachable = new bool[slots.Length];
            Stack<(int, int)> stack = new Stack<(int, int)>();

            // Start with all chunk edges
            for (int a = 0; a < CHUNK_SIZE; a++)
            {
                stack.Push((a, 0));
                stack.Push((a, CHUNK_SIZE - 1));
                stack.Push((0, a));
                stack.Push((CHUNK_SIZE - 1, a));
            }

            while (stack.Count > 0)
            {
                (int x, int y) = stack.Pop();
                if (reachable[y * CHUNK_SIZE + x]) continue;
                reachable[y * CHUNK_SIZE + x] = true;

                Slot slot = slots[y * CHUNK_SIZE + x];

                for (int d = 0; d < Direction.COUNT; d++)
                {
                    if (!slot.module.edges[d]) continue;

                    Slot neighbor = slot.Neighbor(d);
                    if (neighbor == null) continue;

                    stack.Push((neighbor.slotX, neighbor.slotY));
                }
            }

            foreach (Slot slot in slots)
            {
                if (!reachable[slot.slotY * CHUNK_SIZE + slot.slotX])
                {
                    slot.module = WFCGenerator.instance.modules[0];
                }
            }
        }
    }
}