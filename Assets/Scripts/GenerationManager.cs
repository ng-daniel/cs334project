using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using WFC;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;

    public WFCGenerator wfc;
    public GameObject cube;

    [Header("Building Generation Settings")]
    [SerializeField]
    public List<BuildingModule> buildingModulesList;
    [SerializeField]
    public AnimationCurve positiveCorrelationCurve;
    [SerializeField]
    public AnimationCurve negativeCorrelationCurve;
    [SerializeField]
    public int minBuildingsPerChunk = 1;
    [SerializeField]
    public int maxBuildingsPerChunk = 5;
    [SerializeField]
    public int numBuildingLayers = 30; 

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

    //private void Start()
    //{
    //    Chunk chunk = new Chunk(0, 0);
    //        // Generate buildings
    //    chunk.buildingGenerator.GenerateLevels();
    //    chunk.buildingGenerator.DebugDraw();
    //}

    public System.Collections.IEnumerable GenerateChunk(Chunk chunk)
    {
        // Generate buildings
        chunk.buildingGenerator.AddFirstLevel();
        for (float yGridHeight = 0; yGridHeight < numBuildingLayers; yGridHeight++)
        {
            float layerY = chunk.buildingGenerator.buildingMap.Count * 2;
            chunk.buildingGenerator.AddNextLayer(1, layerY);
            yield return null;
        }

        chunk.buildingGenerator.DebugDraw();
        //foreach (var _ in wfc.Generate(chunk))
        //{
        //    yield return null;
        //}
        //foreach (var _ in chunk.PostGeneration())
        //{
        //    yield return null;
        //}
    }

    /// <summary>
    /// Get a random building module from the list, according to module chance type and value.
    /// Pass in the y position (height) of the object.
    /// </summary>
    /// <param name="bottomModule"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public BuildingModule GetRandomBuildingModule(BuildingModule bottomModule, float yPosition)
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

        Assert.Greater(compatibleModules.Count, 0);

        // Use weighted probabilities to assign random module
        float total = 0.0f;
        foreach (BuildingModule module in compatibleModules)
        {
            switch (module.chanceType)
            {
                case BuildingModule.ChanceType.CONSTANT:
                    total += module.chanceValue;
                    break;
                case BuildingModule.ChanceType.POSITIVE_CORRELATION:
                    total += module.chanceValue * positiveCorrelationCurve.Evaluate(yPosition/(numBuildingLayers * 2));
                    break;
                case BuildingModule.ChanceType.NEGATIVE_CORRELATION:
                    total += module.chanceValue * negativeCorrelationCurve.Evaluate(yPosition/(numBuildingLayers * 2));
                    break;
                default:
                    Debug.LogError("Invalid Chance Type.");
                    break;
            }
        }

        // Choose random module from compatible list
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