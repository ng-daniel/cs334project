using UnityEngine;

namespace WFC
{
    public class Slot
    {
        public const float SLOT_SIZE = 2;
        public const float SLOT_HEIGHT = 0.2f;
        public const float SLOT_CENTER = 0.6f;
        public const float SLOT_EDGE = 0.6f;

        public readonly Chunk chunk;
        public readonly int slotX;
        public readonly int slotY;

        // Whether each module is in this slot's superposition
        public readonly bool[] wave;

        // For each module, for each direction, the number of modules
        // that could be in that direction for that module
        // If any direction is 0, the module cannot go in this slot
        public readonly int[][] compatibility;

        public int possibleModuleCount;
        public Module module;

        public Slot(Chunk chunk, int slotX, int slotY)
        {
            this.chunk = chunk;
            this.slotX = slotX;
            this.slotY = slotY;

            wave = new bool[WFCGenerator.instance.ModuleCount()];
            compatibility = new int[WFCGenerator.instance.ModuleCount()][];

            for (int m = 0; m < WFCGenerator.instance.ModuleCount(); m++)
            {
                wave[m] = true;
                compatibility[m] = new int[Direction.COUNT];

                for (int d = 0; d < Direction.COUNT; d++)
                {
                    compatibility[m][d] = WFCGenerator.instance.adjacencies[m][d].Count;
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
            return chunk.level.GetSlot(slotX + Direction.OffsetX(d), slotY + Direction.OffsetY(d));
        }

        public Vector3 WorldPos()
        {
            int x = chunk.chunkX * Chunk.CHUNK_SIZE + slotX;
            int y = chunk.chunkY * Chunk.CHUNK_SIZE + slotY;
            return new Vector3(x * SLOT_SIZE, 0, y * SLOT_SIZE);
        }

        public void Spawn()
        {
            bool empty = true;

            for (int d = 0; d < Direction.COUNT; d++)
            {
                if (module.edges[d])
                {
                    empty = false;
                    break;
                }
            }

            if (empty)
            {
                return;
            }

            GameObject cube = GenerationManager.instance.cube;

            GameObject center = Object.Instantiate(cube);
            center.transform.position = WorldPos();
            center.transform.localScale = new Vector3(SLOT_CENTER, SLOT_HEIGHT, SLOT_CENTER);

            for (int d = 0; d < Direction.COUNT; d++)
            {
                if (module.edges[d])
                {
                    float x = Direction.OffsetX(d) * (SLOT_CENTER + SLOT_EDGE) / 2f;
                    float y = Direction.OffsetY(d) * (SLOT_CENTER + SLOT_EDGE) / 2f;

                    GameObject edge = Object.Instantiate(cube);
                    edge.transform.position = WorldPos() + new Vector3(x, 0f, y);

                    if (d == Direction.LEFT || d == Direction.RIGHT)
                    {
                        edge.transform.localScale = new Vector3(SLOT_EDGE, SLOT_HEIGHT, SLOT_CENTER);
                    }
                    else
                    {
                        edge.transform.localScale = new Vector3(SLOT_CENTER, SLOT_HEIGHT, SLOT_EDGE);
                    }
                }
            }
        }
    }
}
