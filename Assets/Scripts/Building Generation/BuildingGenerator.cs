using NUnit.Framework;
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using WFC;

public class BuildingGenerator
{
    private Chunk chunk;
    private List<BuildingModule> modulesList;

    public List<Level<BuildingSlot>> buildingMap; // A list of levels at various heights
    

    public BuildingGenerator(Chunk chunk)
    {
        this.chunk = chunk;
        this.buildingMap = new List<Level<BuildingSlot>>();
        modulesList = GenerationManager.instance.buildingModulesList;
    }

    //public void GenerateLevels()
    //{
    //    Assert.Greater(GenerationManager.instance.buildingModulesList.Count, 0);
        
    //    AddFirstLevel();

    //    // Currently spacing building layers out evenly TODO: change varying heights
    //    int numLayers = GenerationManager.instance.numBuildingLayers;
    //    for (float yGridHeight = 0; yGridHeight < numLayers; yGridHeight++)
    //    {
    //        float layerY = buildingMap.Count * 2;
    //        AddNextLayer(1, layerY);
    //    }
    //}

    public void AddNextLayer(int levelHeight, float yPosition)
    {
        Level<BuildingSlot> prevLevel = buildingMap[buildingMap.Count - 1];
        Level<BuildingSlot> newLevel = new Level<BuildingSlot>(chunk, levelHeight, yPosition);

        for (int i = 0; i < prevLevel.slots.Length; i++)
        {
            int xPos = prevLevel.GetX(i);
            int yPos = prevLevel.GetY(i);

            // Get the module eof slot below ("old") and new slot
            BuildingModule oldModule = prevLevel.slots[i].buildingModule;

            // Assert oldModule is defined and not null
            //Assert.IsNotNull(oldModule);

            BuildingSlot newSlot = new BuildingSlot(chunk, xPos, yPos, newLevel);

            // Set the building module of the new slot
            newSlot.buildingModule = GenerationManager.instance.GetRandomBuildingModule(oldModule, yPosition);            
            newLevel.slots[i] = newSlot;

        }

        buildingMap.Add(newLevel);
    }

    public void AddFirstLevel()
    {

        Level<BuildingSlot> level = new Level<BuildingSlot>(chunk, 1, 0.0f);

        if (modulesList == null || modulesList.Count == 0)
        {
            Debug.Log("Generation Manager's Building Modules List is empty.");
            return;
        }

        // Temp: randomly fill the building map
        for (int i = 0; i < level.slots.Length; i++)
        {
            level.slots[i] = new BuildingSlot(chunk, level.GetX(i), level.GetY(i), level);

            // Assign empty module
            level.slots[i].buildingModule = modulesList[0]; // empty
        }

        // Random number of "buildings" (rectangles) per chunk
        int numBuildings = Random.Range(GenerationManager.instance.minBuildingsPerChunk,
                                        GenerationManager.instance.maxBuildingsPerChunk + 1);
        for (int i = 0; i < numBuildings; i++)
        {
            int x1 = Random.Range(1, Chunk.CHUNK_SIZE);
            int x2 = Random.Range(1, Chunk.CHUNK_SIZE);

            int y1 = Random.Range(1, Chunk.CHUNK_SIZE);
            int y2 = Random.Range(1, Chunk.CHUNK_SIZE);

            // Swap coordinates if larger
            if (x2 < x1)
            {
                int temp = x2; x2 = x1; x1 = temp;
            }
            if (y2 < y1)
            {
                int temp = y2; y2 = y1; y1 = temp;
            }

            // Fill rect
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    BuildingSlot slot = level.GetSlot(x, y);
                    slot.buildingModule = modulesList[1]; // solid
                }
            }
        }
        
        buildingMap.Add(level);
    }

    public void DebugDraw()
    {
        // For each level
        // Loop through Building Slots and draw
        foreach (Level<BuildingSlot> level in buildingMap)
        {
            //float heightToAdd = -1.0f;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    // Draw the prefab here
                    BuildingSlot slot = level.GetSlot(x, y);
                    
                    slot.Spawn();
                }
            }
        }
    }
    
}
