using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using WFC;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;

    public WFCGenerator wfc;
    public GameObject cube;

    [SerializeField]
    public List<BuildingModule> buildingModulesList;

    [SerializeField]
    public int numRectsInFirstLevel = 2; // Num of rectangles filled in first layer per chunk

    void Awake()
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

    public IEnumerable GenerateChunk(Chunk chunk)
    {
        // Generate buildings
        chunk.buildingGenerator.GenerateLevels();
        chunk.buildingGenerator.DebugDraw();

        foreach (var _ in wfc.Generate(chunk))
        {
            yield return null;
        }
        foreach (var _ in chunk.PostGeneration())
        {
            yield return null;
        }
    }

    public BuildingModule GetRandomBuildingModule(BuildingModule bottomModule)
    {
        if (bottomModule == null) return null;

        // Get a list of all compatible modules
        List<BuildingModule> compatibleModules = new List<BuildingModule>();

        foreach (BuildingModule module in buildingModulesList)
        {
            if (module.CanStackOnType(bottomModule.modelType))
            {
                compatibleModules.Add(module);
            }
        }

        Assert.Greater(compatibleModules.Count, 0);

        // Choose random from list TODO: use weighted probs, get probs from height
        // Assign random module
        int randInd = Random.Range(0, compatibleModules.Count);
        return compatibleModules[randInd];
    }
}