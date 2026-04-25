using System.Collections;
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

        public IEnumerable Load()
        {
            return GenerationManager.instance.GenerateChunk(pathChunk);
        }

        public IEnumerable Unload()
        {
            foreach (Slot slot in pathChunk.level.slots)
            {
                slot.Unload();
                yield return null;
            }
        }
    }
}