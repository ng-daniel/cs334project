using System.Numerics;
using WFC;

public class Level<T>
{
    public readonly Chunk chunk;

    public T[] slots;
    public int levelGridHeight = 0;      // size (height) of level
    public float levelYPosition = 0.0f; // y position of level (height above ground)

    public Level(WFC.Chunk chunk, int levelGridHeight, float levelYPosition)
    {
        this.chunk = chunk;
        this.slots = new T[Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE];

        this.levelGridHeight = levelGridHeight;
        this.levelYPosition = levelYPosition;
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