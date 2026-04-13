using UnityEngine;
using WFC;

namespace Assets.Scripts.ChunkLoading
{
    [System.Serializable]
    public class ChunkNode
    {
        [SerializeField] Vector2Int coords;
        [SerializeField] Chunk pathChunk;
        public ChunkNode(Vector2Int coords)
        {
            this.coords = coords;
            pathChunk = new Chunk(coords.x, coords.y);
        }

        public void Load()
        {
            GenerationManager.instance.GenerateChunk(pathChunk);
        }

        public void Unload()
        {
            // Implementation for unloading the chunk (e.g., destroy GameObjects, clean up data structures, etc.)
        }
    }
}