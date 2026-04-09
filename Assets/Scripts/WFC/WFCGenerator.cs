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
using Random = UnityEngine.Random;

namespace WFC
{
    public class WFCGenerator
    {
        public static WFCGenerator instance;

        public List<Module> modules;

        // For each module, for each direction, the list of modules that
        // can be in that direction from that module
        // (called "propagator" in reference code)
        public List<int>[][] adjacencies;

        public Stack<(Slot, int)> stack;

        public WFCGenerator()
        {
            instance = this;

            modules = new List<Module>();
        }

        public void AddModule(string name, int bitmap, int symmetry)
        {
            bool[] edges = new bool[Direction.COUNT];
            for (int i = 0; i < edges.Length; i++)
            {
                edges[edges.Length - i - 1] = (bitmap & 1) != 0;
                bitmap >>= 1;
            }

            for (int angle = 0; angle < symmetry; angle += 90)
            {
                modules.Add(new Module(modules.Count, edges, name, angle));

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
            adjacencies = new List<int>[ModuleCount()][];
            for (int m = 0; m < ModuleCount(); m++)
            {
                adjacencies[m] = new List<int>[Direction.COUNT];
                for (int d = 0; d < Direction.COUNT; d++)
                {
                    adjacencies[m][d] = new List<int>();
                    for (int m2 = 0; m2 < ModuleCount(); m2++)
                    {
                        // If you have module m, then go in direction d, can module m2 be there?
                        if (modules[m].edges[d] == modules[m2].edges[Direction.Opposite(d)])
                        {
                            adjacencies[m][d].Add(m2);
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
                if (slot.wave[m] != (m == module))
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

                    foreach (int m2 in adjacencies[m][d])
                    {
                        int o = Direction.Opposite(d);

                        // If the neighbor slot has module m2, one of the modules
                        // in adjacencies[m2][opposite(d)] must be in the source slot.
                        // One such case has been ruled out.
                        neighbor.compatibility[m2][o]--;

                        if (neighbor.compatibility[m2][o] == 0)
                        {
                            neighbor.Remove(m2);
                            stack.Push((neighbor, m2));

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
    }
}