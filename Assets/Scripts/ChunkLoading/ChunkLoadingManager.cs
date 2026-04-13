using System.Collections.Generic;
using UnityEngine;
using WFC;

namespace Assets.Scripts.ChunkLoading
{
    public class ChunkLoadingManager : MonoBehaviour
    {
        Dictionary<Vector2Int, ChunkNode> activeChunks = new Dictionary<Vector2Int, ChunkNode>();
        Vector2Int playerChunkCoords;
        int chunkLoadRadius = 2; // How many chunks away from the player should be loaded
        int chunkUnloadRadius = 3; // How many chunks away from the player should be unloaded

        public void Start()
        {
            InitializeChunks(this.transform);
        }

        /// <summary>
        /// Initializes the chunk loading system given a starting transform and loads the initial set of chunks around it.
        /// </summary>
        /// <param name="startingTransform">The transform to use as the starting point for chunk loading.</param>
        public void InitializeChunks(Transform startingTransform)
        {
            playerChunkCoords = ChunkLoadingHelper.GetChunkCoords(startingTransform.position);
            List<Vector2Int> initialChunks = ChunkLoadingHelper.GetChunkCoordsInRadius(playerChunkCoords, 1);

            foreach (Vector2Int chunkCoords in initialChunks)
            {
                LoadChunk(chunkCoords);
            }
        }

        public void OnPlayerCoordsChanged(Vector3 newPosition)
        {
            Vector2Int newChunkCoords = ChunkLoadingHelper.GetChunkCoords(newPosition);
            if (newChunkCoords != playerChunkCoords)
            {
                UpdateChunks(playerChunkCoords, newChunkCoords);
                playerChunkCoords = newChunkCoords;
            }
        }

        private void LoadChunk(Vector2Int chunkCoords)
        {
            if (!activeChunks.ContainsKey(chunkCoords))
            {
                ChunkNode chunkNode = new(chunkCoords);
                chunkNode.Load();
                activeChunks.Add(chunkCoords, chunkNode);
            }
        }

        private void UnloadChunk(Vector2Int chunkCoords)
        {
            if (activeChunks.ContainsKey(chunkCoords))
            {
                ChunkNode chunkNode = activeChunks[chunkCoords];
                chunkNode.Unload();
                activeChunks.Remove(chunkCoords);
            }
        }

        private void UpdateChunks(Vector2Int oldPlayerChunkCoords, Vector2Int newPlayerChunkCoords)
        {
            // Implementation for updating chunks based on player movement
            List<Vector2Int> newChunks = ChunkLoadingHelper.GetChunkCoordsInRadius(newPlayerChunkCoords, chunkLoadRadius);
            List<Vector2Int> oldChunks = ChunkLoadingHelper.GetChunkCoordsInRadius(oldPlayerChunkCoords, chunkUnloadRadius);

            List<Vector2Int> chunksToLoad = ChunkLoadingHelper.GetChunksToLoad(oldChunks, newChunks);
            List<Vector2Int> chunksToUnload = ChunkLoadingHelper.GetChunksToUnload(oldChunks, newChunks);

            foreach (Vector2Int chunkCoords in chunksToLoad)
            {
                LoadChunk(chunkCoords);
            }

            foreach (Vector2Int chunkCoords in chunksToUnload)
            {
                UnloadChunk(chunkCoords);
            }
        }
    }
}