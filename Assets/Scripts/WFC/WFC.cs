using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class WFC : MonoBehaviour
    {
        public static WFC instance;

        public GameObject cube;

        public float tileScale = 1;
        public float tileHeight = 0.2f;

        public List<Module> modules;

        void Start()
        {
            instance = this;

            modules = new List<Module>();
            modules.Add(new Module(false, false, false, false));
            modules.Add(new Module(true, false, false, false));
            modules.Add(new Module(true, true, false, false));
            modules.Add(new Module(true, true, true, false));
            modules.Add(new Module(true, true, true, true));

            Chunk chunk = new Chunk(0, 0);
            Generate(chunk);
        }

        public int ModuleCount()
        {
            return modules.Count;
        }

        public bool Generate(Chunk chunk)
        {
            while (true)
            {
                Slot slot = NextSlot(chunk);
                if (slot == null)
                {
                    break;
                }

                Collapse(slot);
                if (!Propagate())
                {
                    return false;
                }
            }

            return true;
        }

        public Slot NextSlot(Chunk chunk)
        {
            Slot min = null;
            int minPossibleCount = int.MaxValue;

            // TODO: Use a min heap for this
            for (int i = 0; i < chunk.slots.Length; i++)
            {
                Slot slot = chunk.slots[i];
                int count = slot.possibleModuleCount;

                if (count <= 1)
                {
                    // Already collapsed
                    continue;
                }

                if (count < minPossibleCount)
                {
                    min = slot;
                    minPossibleCount = count;
                }
            }

            return min;
        }

        public void Collapse(Slot slot)
        {
            float rand = Random.value * slot.possibleModuleCount;
            int module = -1;

            for (int m = 0; m < slot.wave.Length; m++)
            {
                if (slot.wave[m])
                {
                    rand--;
                    if (rand <= 0)
                    {
                        module = m;
                        break;
                    }
                }
            }

            for (int m = 0; m < slot.wave.Length; m++)
            {
                if (m != module)
                {
                    slot.Remove(m);
                }
            }
        }

        public bool Propagate()
        {

        }

        private void PlaceCube(int x, int y)
        {
            GameObject go = Instantiate(cube);
            go.transform.position = new Vector3(x * tileScale, 0, y * tileScale);
            go.transform.localScale = new Vector3(tileScale, tileHeight, tileScale);
        }
    }
}