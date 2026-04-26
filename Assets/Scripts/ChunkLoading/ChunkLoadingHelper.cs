using System;
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

        /// <summary>
        /// Computes the heights of each layer for a chunk, starting from the ground and working upwards. 
        /// 
        /// The number of layers and the height of each layer is determined by the parameters set in the inspector. 
        /// The height of each layer will not exceed maxHeight, and each layer will have a height that is at least minHeight. 
        /// 
        /// The heights of the layers are also influenced by the buildingPieceHeights list, which contains possible heights 
        /// for building pieces that can be used to fill the layers. Each layer must have a height that can be filled by a 
        /// combination of building pieces from the buildingPieceHeights list.
        /// 
        /// This function ensures that the generated layers are varied and fit within the specified height constraints.
        /// </summary>
        /// <returns>A list where each element is a tuple containing the height of the layer and a list of building piece heights that make up that layer.</returns>
        public static List<Tuple<int, List<int>>> ComputeLayerHeights(int numLayers, int maxTotalHeight, int minLayerHeight, int maxLayerHeight, List<int> buildingPieceHeights)
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks); // Ensure different random sequence each time
            
            List<Tuple<int, List<int>>> layerHeights = new List<Tuple<int, List<int>>>();
            int remainingHeight = maxTotalHeight;

            for (int i = 0; i < numLayers; i++)
            {
                int clampedMaxHeight = Mathf.Min(maxLayerHeight, remainingHeight);
                int layerHeight = UnityEngine.Random.Range(minLayerHeight, clampedMaxHeight + 1);
                List<int> buildingPieces = GetRandomSequenceSummingTo(layerHeight, buildingPieceHeights);
                if (buildingPieces == null)
                {
                    // try again if fail
                    Debug.LogError($"Cannot generate layer {i + 1} with height {layerHeight} using available building piece heights.");
                    i--;
                    continue;
                }
                layerHeights.Add(Tuple.Create(layerHeight, buildingPieces));
                remainingHeight -= layerHeight;
            }

            return layerHeights;
        }

        /// <summary>
        /// Returns a random sequence of integers sampled with replacement from values
        /// whose sum equals target or null if no such sequence exists.
        /// </summary>
        /// <param name="target">The required sum.</param>
        /// <param name="values">The pool of integers to sample from.</param>
        /// <returns>A list of integers summing to target, or null if impossible.</returns>
        public static List<int> GetRandomSequenceSummingTo(int target, List<int> values)
        {
            if (values == null || values.Count == 0 || target <= 0)
                return null;

            // build a reachability table
            // dp[i] = true if sum i is reachable using elements from values
            bool[] dp = new bool[target + 1];
            dp[0] = true;
            for (int i = 1; i <= target; i++)
            {
                foreach (int v in values)
                {
                    if (v <= i && dp[i - v])
                    {
                        dp[i] = true;
                        break;
                    }
                }
            }

            if (!dp[target])
                return null;

            List<int> result = new List<int>();
            int remaining = target;

            while (remaining > 0)
            {
                // Collect only candidates that keep a valid solution reachable
                List<int> candidates = new List<int>();
                foreach (int v in values)
                {
                    if (v <= remaining && dp[remaining - v])
                        candidates.Add(v);
                }

                int chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                result.Add(chosen);
                remaining -= chosen;
            }

            return result;
        }


    }
}