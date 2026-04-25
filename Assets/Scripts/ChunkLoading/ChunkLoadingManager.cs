using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using WFC;

namespace Assets.Scripts.ChunkLoading
{
    public class ChunkLoadingManager : MonoBehaviour
    {
        public static int CHUNK_SIZE_RAW = Chunk.CHUNK_SIZE * (int)Slot.SLOT_SIZE;
        public static ChunkLoadingManager instance;
        Dictionary<Vector2Int, ChunkNode> activeChunks = new Dictionary<Vector2Int, ChunkNode>();
        Vector2Int playerChunkCoords;
        private bool runningCoroutine;
        [SerializeField] int initialChunkRadius = 2; // Initial chunks within this radius will be loaded on start
        [SerializeField] int chunkLoadRadius = 2; // Empty chunks at this radius or closer will be loaded
        [SerializeField] int chunkUnloadRadius = 3; // Chunks more than this value away from the player will be unloaded

        GameObject player;

        public void Start()
        {
            instance = this;

            try
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().gameObject;
                InitializeChunks(player.transform);
            }
            catch
            {
                Debug.LogError("PlayerController / Player Prefab not found in scene.");
                InitializeChunks(this.transform);
            }
        }

        /// <summary>
        /// Initializes the chunk loading system given a starting transform and loads the initial set of chunks around it.
        /// </summary>
        /// <param name="startingTransform">The transform to use as the starting point for chunk loading.</param>
        public void InitializeChunks(Transform startingTransform)
        {
            playerChunkCoords = ChunkLoadingHelper.GetChunkCoords(startingTransform.position);
            AsyncRunner.RunSync(UpdateChunks(playerChunkCoords, playerChunkCoords));
        }

        public void Update()
        {
            if (player != null)
            {
                OnPlayerCoordsChanged(player.transform.position);
            }
        }

        public void OnPlayerCoordsChanged(Vector3 newPosition)
        {
            Vector2Int newChunkCoords = ChunkLoadingHelper.GetChunkCoords(newPosition);
            if (newChunkCoords != playerChunkCoords)
            {
                Vector2Int oldChunkCoords = playerChunkCoords;
                playerChunkCoords = newChunkCoords;
                AsyncRunner.RunAsync(UpdateChunks(oldChunkCoords, newChunkCoords));
            }
        }

        private IEnumerable UpdateChunks(Vector2Int oldPlayerChunkCoords, Vector2Int newPlayerChunkCoords)
        {
            // Implementation for updating chunks based on player movement
            List<Vector2Int> loadMaybe = ChunkLoadingHelper.GetChunkCoordsInRadius(newPlayerChunkCoords, chunkLoadRadius);
            List<Vector2Int> chunksToKeep = ChunkLoadingHelper.GetChunkCoordsInRadius(oldPlayerChunkCoords, chunkUnloadRadius);
            print(loadMaybe.Count);

            // only unload chunks that are actively loaded and not in the keep radius
            // only load chunks that are in loadMaybe and not actively loaded, and
            int n_loaded = 0;
            int n_unloaded = 0;
            List<Vector2Int> activeChunkCoords = new List<Vector2Int>(activeChunks.Keys);
            for (int i = 0; i < activeChunkCoords.Count; i++)
            {
                if (!chunksToKeep.Contains(activeChunkCoords[i]))
                {
                    n_unloaded++;
                    yield return UnloadChunk(activeChunkCoords[i]);
                }
            }
            for (int i = 0; i < loadMaybe.Count; i++)
            {
                if (!activeChunks.ContainsKey(loadMaybe[i]))
                {
                    n_loaded++;
                    yield return LoadChunk(loadMaybe[i]);
                }
            }

            print($"UpdateChunks Result: {n_loaded} chunks loaded, {n_unloaded} chunks unloaded, {activeChunks.Count} total active chunks");
        }

        private IEnumerable LoadChunk(Vector2Int chunkCoords)
        {
            if (!activeChunks.ContainsKey(chunkCoords))
            {
                ChunkNode chunkNode = new(chunkCoords);
                activeChunks.Add(chunkCoords, chunkNode);
                yield return chunkNode.Load();
            }
        }
        private IEnumerable UnloadChunk(Vector2Int chunkCoords)
        {
            if (activeChunks.ContainsKey(chunkCoords))
            {
                ChunkNode chunkNode = activeChunks[chunkCoords];
                activeChunks.Remove(chunkCoords);
                yield return chunkNode.Unload();
            }
        }

        public void OnDrawGizmos()
        {
            // Draw wire cubes around loaded chunks for debugging
            Gizmos.color = Color.green;
            foreach (Vector2Int chunkCoords in activeChunks.Keys)
            {
                Vector3 chunkCenter = new Vector3(chunkCoords.x * CHUNK_SIZE_RAW + CHUNK_SIZE_RAW / 2f, 0, chunkCoords.y * CHUNK_SIZE_RAW + CHUNK_SIZE_RAW / 2f);
                Gizmos.DrawWireCube(chunkCenter, new Vector3(CHUNK_SIZE_RAW, 1, CHUNK_SIZE_RAW));
            }
        }

    }
}