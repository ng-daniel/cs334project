using UnityEngine;
using WFC;

public class GenerationManager : MonoBehaviour
{
    public WFCGenerator wfc;

    void Start()
    {
        wfc = new WFCGenerator();

        wfc.AddModule(null, 0b0000, 90, 4f);
        wfc.AddModule("Endpoint", 0b1000, 360, 0.1f);
        wfc.AddModule("Corner", 0b1100, 360, 1f);
        wfc.AddModule("Straight", 0b1010, 180, 5f);
        wfc.AddModule("Junction", 0b1110, 360, 0.1f);
        wfc.AddModule("Cross", 0b1111, 90, 0.05f);

        wfc.BuildAdjacencies();

        Chunk chunk = new Chunk(0, 0);
        wfc.Generate(chunk);
        chunk.PostGeneration();
    }
}