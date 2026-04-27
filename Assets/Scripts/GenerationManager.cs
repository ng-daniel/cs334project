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
        chunk.buildingGenerator.BuildHollowMap();

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

    
}