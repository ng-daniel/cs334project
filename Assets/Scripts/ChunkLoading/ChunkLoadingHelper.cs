using System.Collections.Generic;
using UnityEngine;
using WFC;

namespace Assets.Scripts.ChunkLoading
{
    public static class ChunkLoadingHelper
    {
        /// <summary>
        /// Convert a world position to chunk coordinates. 
        /// The chunk coordinates are based on the chunk size defined in the Chunk class.
        /// </summary>
        /// <param name="position">The world position to convert.</param>
        /// <returns>The chunk coordinates as a Vector2Int.</returns>
        public static Vector2Int GetChunkCoords(Vector3 position)
        {
            int chunkX = Mathf.FloorToInt(position.x / ChunkLoadingManager.CHUNK_SIZE_RAW);
            int chunkY = Mathf.FloorToInt(position.z / ChunkLoadingManager.CHUNK_SIZE_RAW);
            return new Vector2Int(chunkX, chunkY);
        }

        /// <summary>
        /// Get a list of chunk coordinates within a certain radius of a center chunk.
        /// Radius is like a manhattan distance, so it will create a diamond-shaped area around the center chunk.
        /// </summary>
        /// <param name="centerChunkCoords">The coordinates of the center chunk.</param>
        /// <param name="radius">The radius around the center chunk to include.</param>
        /// <returns>A list of chunk coordinates within the specified radius.</returns>
        public static List<Vector2Int> GetChunkCoordsInRadius(Vector2Int centerChunkCoords, int radius)
        {
            List<Vector2Int> chunksInRange = new List<Vector2Int>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    // Skip chunks outside of the radius (diamond shape)
                    if (Mathf.Abs(x) + Mathf.Abs(y) > radius) continue;

                    Vector2Int chunkCoords = new Vector2Int(centerChunkCoords.x + x, centerChunkCoords.y + y);
                    chunksInRange.Add(chunkCoords);
                }
            }
            return chunksInRange;
        }
    }
}