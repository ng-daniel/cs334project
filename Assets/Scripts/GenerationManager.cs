using UnityEngine;
using WFC;

public class GenerationManager : MonoBehaviour
{
    public WFCGenerator wfc;

    void Start()
    {
        wfc = new WFCGenerator();

        wfc.AddModule(null, 0b0000, 90);
        wfc.AddModule("Endpoint", 0b1000, 360);
        wfc.AddModule("Corner", 0b1100, 360);
        wfc.AddModule("Straight", 0b1010, 180);
        wfc.AddModule("Junction", 0b1110, 360);
        wfc.AddModule("Cross", 0b1111, 90);

        wfc.BuildAdjacencies();

        Chunk chunk = new Chunk(0, 0);
        wfc.Generate(chunk);
        chunk.PostGeneration();
    }
}