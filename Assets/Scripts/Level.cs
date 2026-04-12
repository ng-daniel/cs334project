using System.Numerics;
using WFC;

public class Level<T>
{
    public readonly Chunk chunk;

    public T[] slots;
    public float levelHeight = 0; // height of the level itself

    public Level(WFC.Chunk chunk, int chunkSize, float levelHeight)
    {
        this.chunk = chunk;
        this.slots = new T[chunkSize * chunkSize];

        this.levelHeight = levelHeight;
    }
    public T GetSlot(int x, int y)
    {
        if (x < 0 || x >= WFC.Chunk.CHUNK_SIZE || y < 0 || y >= WFC.Chunk.CHUNK_SIZE)
        {
            return default(T);
        }
        return slots[y * WFC.Chunk.CHUNK_SIZE + x];
    }

    public int GetX(int slotIndex)
    {
        return slotIndex % WFC.Chunk.CHUNK_SIZE;
    }

    public int GetY(int slotIndex)
    {
        return slotIndex / WFC.Chunk.CHUNK_SIZE;
    }
}