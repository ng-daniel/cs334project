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

            for (int i = 0; i < ChunkLoadingManager.instance.LayerHeights.Count; i++)
            {
                int height = ChunkLoadingManager.instance.LayerHeights[i];
                chunkLayers.Add(new Chunk(coords.x, coords.y, height));

                if (i == 0)
                {
                    basePathChunk = chunkLayers[0];
                }
            }

            this.buildingGenerator = new BuildingGenerator(basePathChunk);
        }

        public IEnumerable Load()
        {
            foreach (Chunk chunk in chunkLayers)
            {
                yield return GenerationManager.instance.GenerateChunk(chunk);
            }
            yield return buildingGenerator.GenerateLevels();
            yield return buildingGenerator.DebugDraw();
        }

        public IEnumerable Unload()
        {
            foreach (Chunk chunk in chunkLayers)
            {
                foreach (Slot slot in chunk.level.slots)
                {
                    slot.Unload();
                    yield return null;
                }
            }

            foreach (Level<BuildingSlot> level in buildingGenerator.buildingMap)
            {
                foreach (BuildingSlot slot in level.slots)
                {
                    slot.Unload();
                    GameObject.Destroy(slot.instantiatedPrefab);
                    slot.instantiatedPrefab = null;
                    yield return null;
                }
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