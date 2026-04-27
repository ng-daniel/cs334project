using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFC;

namespace Assets.Scripts.ChunkLoading
{
    [Serializable]
    public class ChunkNode
    {
        [SerializeField] Vector2Int coords;
        [SerializeField] Chunk basePathChunk;
        [SerializeField] List<Chunk> chunkLayers = new List<Chunk>();
        [SerializeField] BuildingGenerator buildingGenerator;
        public ChunkNode(Vector2Int coords)
        {
            this.coords = coords;
            basePathChunk = new Chunk(coords.x, coords.y);

            for (int i = 0; i < ChunkLoadingManager.instance.layerData.Count; i++)
            {
                chunkLayers.Add(new Chunk(coords.x, coords.y));
            }

            this.buildingGenerator = new BuildingGenerator(basePathChunk);
        }

        public IEnumerable Load()
        {
            foreach (Tuple<int, List<int>> layer in ChunkLoadingManager.instance.layerData)
            {
                int layerHeight = layer.Item1;
            }
            yield return GenerationManager.instance.GenerateChunk(this);

            yield return buildingGenerator.GenerateLevels();
            yield return buildingGenerator.DebugDraw();
        }

        public IEnumerable Unload()
        {
            foreach (Slot slot in basePathChunk.level.slots)
            {
                slot.Unload();
                yield return null;
            }
        }

        public Chunk GetPathChunk()
        {
            return basePathChunk;
        }

        public BuildingGenerator GetBuildingGenerator()
        {
            return buildingGenerator;
        }
    }
}