using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

    public void GenerateChunk(Chunk chunk)
    {
        // Generate buildings
        chunk.buildingGenerator.GenerateLevels();
        chunk.buildingGenerator.DebugDraw();

        wfc.Generate(chunk);
        chunk.PostGeneration();
    }

    public BuildingModule GetRandomBuildingModule(BuildingModule bottomModule)
    {
        if (bottomModule == null) return null;
        
        // Get a list of all compatible modules
        List <BuildingModule> compatibleModules = new List <BuildingModule>();

        foreach (BuildingModule module in buildingModulesList)
        {
            if (module.CanStackOnType(bottomModule.modelType))
            {
                compatibleModules.Add(module);
            }
        }

        //Assert.Greater(compatibleModules.Count, 0);

        // Use weighted probabilities to assign random module
        float total = 0.0f;
        foreach (BuildingModule module in compatibleModules)
        {
            total += module.chanceValue;
            // TODO: modify chance value based on chance type
        }

        float randVal = Random.value * total;
        foreach (BuildingModule module in compatibleModules)
        {
            if (randVal < module.chanceValue)
            {
                return module;
            } else
            {
                randVal -= module.chanceValue;
            }
        }
        return compatibleModules[0];

    }
}