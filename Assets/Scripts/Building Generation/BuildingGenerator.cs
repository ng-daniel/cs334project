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
    private const int NUM_LAYERS = 50;

    public List<Level<BuildingSlot>> buildingMap; // A list of levels at various heights
    public BuildingGenerator(Chunk chunk)
    {
        this.chunk = chunk;
        this.buildingMap = new List<Level<BuildingSlot>>();
    }

    public void GenerateLevels()
    {
        Assert.Greater(GenerationManager.instance.buildingModulesList.Count, 0);
        
        AddFirstLevel();

        // Currently spacing building layers out evenly
        // TODO create a number of layers parameter
        for (float yHeight = 0; yHeight < NUM_LAYERS; yHeight++)
        {
            AddNextLayer(1);
            // TODO change level height
        }
    }

    private void AddNextLayer(float zPos)
    {
        Level<BuildingSlot> prevLevel = buildingMap[buildingMap.Count - 1];
        Level<BuildingSlot> newLevel = new Level<BuildingSlot>(chunk, 1);

        for (int i = 0; i < prevLevel.slots.Length; i++)
        {
            int xPos = prevLevel.GetX(i);
            int yPos = prevLevel.GetY(i);

            // Get the module eof slot below ("old") and new slot
            BuildingModule oldModule = prevLevel.slots[i].buildingModule;

            // Assert oldModule is defined and not null
            //Assert.IsNotNull(oldModule);

            BuildingSlot newSlot = new BuildingSlot(chunk, xPos, yPos);

            // Set the building module of the new slot
            newSlot.buildingModule = GenerationManager.instance.GetRandomBuildingModule(oldModule);            
            newLevel.slots[i] = newSlot;
        }

        // TODO: consider adding floating objects with a random chance

        buildingMap.Add(newLevel);
    }

    private void AddFirstLevel()
    {

        Level<BuildingSlot> level = new Level<BuildingSlot>(chunk, 1);
        List<BuildingModule> modulesList = GenerationManager.instance.buildingModulesList;

        if (modulesList == null || modulesList.Count == 0)
        {
            Debug.Log("Generation Manager's Building Modules List is empty.");
            return;
        }

        // Temp: randomly fill the building map
        for (int i = 0; i < level.slots.Length; i++)
        {
            level.slots[i] = new BuildingSlot(chunk, level.GetX(i), level.GetY(i));

            // Assign empty module
            level.slots[i].buildingModule = modulesList[0]; // empty
        }

        for (int i = 0; i < GenerationManager.instance.numRectsInFirstLevel; i++)
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
        float yHeight = 0.0f;
        foreach (Level<BuildingSlot> level in buildingMap)
        {
            float heightToAdd = -1.0f;
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    // Draw the prefab here
                    BuildingSlot slot = level.GetSlot(x, y);
                    
                    slot.Spawn();

                    // If not empty air
                    if (slot.instantiatedPrefab != null)
                    {
                        // TODO make cleaner AAHH
                        Vector3 pos = slot.WorldPos();
                        pos.y = yHeight;
                        slot.instantiatedPrefab.transform.position = pos;

                        // Add height
                        heightToAdd = Mathf.Max(heightToAdd,
                            slot.instantiatedPrefab.transform.localScale.y);
                        
                    }

                }
            }
            // Update height
            yHeight += heightToAdd;
        }
    }
    
}
