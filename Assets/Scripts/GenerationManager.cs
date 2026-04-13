using UnityEngine;
using WFC;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;

    public WFCGenerator wfc;
    public GameObject cube;

    void Start()
    {
        instance = this;
        wfc = new WFCGenerator();

        wfc.AddModule("Empty", 0b0000, 90, 4f);
        wfc.AddModule("Endpoint", 0b1000, 360, 0.1f);
        wfc.AddModule("Corner", 0b1100, 360, 1f);
        wfc.AddModule("Straight", 0b1010, 180, 5f);
        wfc.AddModule("Junction", 0b1110, 360, 0.1f);
        wfc.AddModule("Cross", 0b1111, 90, 0.05f);

        wfc.BuildAdjacencies();
    }

    public void GenerateChunk(Chunk chunk)
    {
        wfc.Generate(chunk);
        chunk.PostGeneration();
    }
}