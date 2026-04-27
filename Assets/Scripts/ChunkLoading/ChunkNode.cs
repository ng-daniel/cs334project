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
        [SerializeField] Chunk pathChunk;
        [SerializeField] BuildingGenerator buildingGenerator;
        public ChunkNode(Vector2Int coords)
        {
            this.coords = coords;
            pathChunk = new Chunk(coords.x, coords.y);
            this.buildingGenerator = new BuildingGenerator(pathChunk);
        }

        public IEnumerable Load()
        {
            foreach (Tuple<int, List<int>> layer in ChunkLoadingManager.instance.layerData)
            {
                int layerHeight = layer.Item1;
            }
            return GenerationManager.instance.GenerateChunk(this);
        }

        public IEnumerable Unload()
        {
            foreach (Slot slot in pathChunk.level.slots)
            {
                slot.Unload();
                yield return null;
            }
        }

        public Chunk GetPathChunk()
        {
            return pathChunk;
        }

        public BuildingGenerator GetBuildingGenerator()
        {
            return buildingGenerator;
        }
    }
}