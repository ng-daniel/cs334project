// Based off https://github.com/mxgmn/WaveFunctionCollapse.
// 
// Copyright(c) 2016 Maxim Gumin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Provided image samples and tiles are not part of WaveFunctionCollapse software.

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WFC
{
    public class WFC : MonoBehaviour
    {
        public static WFC instance;

        public GameObject cube;

        public float tileScale = 1;
        public float tileHeight = 0.2f;

        public List<Module> modules;

        // For each direction, for each module, the list of modules that
        // can be in that direction from that module
        // (called "propagator" in reference code)
        public List<int>[][] adjacencies;

        public Stack<(Slot, int)> stack;

        void Start()
        {
            instance = this;

            modules = new List<Module>();
            AddModule(0b0000, 90);
            AddModule(0b1000, 360);
            AddModule(0b1100, 360);
            AddModule(0b1010, 180);
            AddModule(0b1110, 360);
            AddModule(0b1111, 90);

            BuildAdjacencies();

            Chunk chunk = new Chunk(0, 0);
            Generate(chunk);
        }

        private void AddModule(int bitmap, int symmetry)
        {
            bool[] edges = new bool[Direction.COUNT];
            for (int i = 0; i < edges.Length; i++)
            {
                edges[edges.Length - i - 1] = (bitmap & 1) != 0;
                bitmap >>= 1;
            }

            for (float angle = 0; angle < symmetry; angle += 90)
            {
                modules.Add(new Module(modules.Count, edges, angle));

                bool temp = edges[0];
                for (int i = 0; i < edges.Length - 1; i++)
                {
                    edges[i] = edges[i + 1];
                }
                edges[^1] = temp;
            }
        }

        public int ModuleCount()
        {
            return modules.Count;
        }

        public void BuildAdjacencies()
        {
            adjacencies = new List<int>[Direction.COUNT][];
            for (int d = 0; d < Direction.COUNT; d++)
            {
                adjacencies[d] = new List<int>[ModuleCount()];
                for (int m = 0; m < ModuleCount(); m++)
                {
                    adjacencies[d][m] = new List<int>();
                    for (int m2 = 0; m2 < ModuleCount(); m2++)
                    {
                        // If you have module m, then go in direction d, can module m2 be there?
                        if (modules[m].edges[d] == modules[m2].edges[Direction.Opposite(d)])
                        {
                            adjacencies[d][m].Add(m2);
                        }
                    }
                }
            }
        }

        public bool Generate(Chunk chunk)
        {
            stack = new Stack<(Slot, int)>();

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

            foreach (Slot slot in chunk.slots)
            {
                for (int m = 0; m < ModuleCount(); m++)
                {
                    if (slot.wave[m])
                    {
                        slot.module = modules[m];
                        break;
                    }
                }

                Spawn(slot.module, slot);
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
                    stack.Push((slot, m));
                }
            }
        }

        public bool Propagate()
        {
            while (stack.Count > 0)
            {
                // It was determined that this module CANNOT go in this slot
                (Slot slot, int m) = stack.Pop();

                for (int d = 0; d < Direction.COUNT; d++)
                {
                    Slot neighbor = slot.Neighbor(d);
                    if (neighbor == null)
                    {
                        continue;
                    }

                    foreach (int m2 in adjacencies[d][m])
                    {
                        // If the neighbor slot has module m2, one of the modules
                        // in adjacencies[o][m2] must be in the source slot.
                        // One such case has been ruled out.
                        neighbor.compatibility[m2][d]--;

                        if (neighbor.compatibility[m2][d] == 0)
                        {
                            neighbor.Remove(m2);
                            if (neighbor.possibleModuleCount == 0)
                            {
                                // No more possible modules, so generation failed
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void Spawn(Module module, Slot slot)
        {
            int x = slot.chunk.chunkX * Chunk.CHUNK_SIZE + slot.x;
            int y = slot.chunk.chunkY * Chunk.CHUNK_SIZE + slot.y;

            GameObject go = Instantiate(cube);
            go.transform.position = new Vector3(x * tileScale, 0, y * tileScale);
            go.transform.rotation = Quaternion.Euler(0, module.angle, 0);
            go.transform.localScale = new Vector3(tileScale, tileHeight, tileScale);
        }
    }
}