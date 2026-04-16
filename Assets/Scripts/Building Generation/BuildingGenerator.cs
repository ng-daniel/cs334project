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
        //for (float zPos = BuildingSlot.SLOT_SIZE; zPos < Chunk.CHUNK_SIZE; zPos += BuildingSlot.SLOT_HEIGHT)
        //{
            //AddNextLayer(BuildingSlot.SLOT_SIZE);
        //}
    }

    private void AddNextLayer(float zPos)
    {
        Level<BuildingSlot> prevLevel = buildingMap[buildingMap.Count - 1];
        Level<BuildingSlot> newLevel = new Level<BuildingSlot>(chunk, zPos);

        for (int i = 0; i < prevLevel.slots.Length; i++)
        {
            int xPos = prevLevel.GetX(i);
            int yPos = prevLevel.GetY(i);

            // Get the module eof slot below ("old") and new slot
            BuildingModule oldModule = prevLevel.slots[i].buildingModule;

            // Assert oldModule is defined and not null
            Assert.IsNotNull(oldModule);

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

        Level<BuildingSlot> level = new Level<BuildingSlot>(chunk, 0.0f);
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

            // Assign random module
            int randInd = Random.Range(0, modulesList.Count);
            level.slots[i].buildingModule = modulesList[randInd];
        }
        
        buildingMap.Add(level);
    }

    public void DebugDraw()
    {
        // For each level
        foreach (Level<BuildingSlot> level in buildingMap)
        {
            // Loop through Building Slots and draw
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    // Draw the prefab here
                    BuildingSlot slot = level.GetSlot(x, y);
                    slot?.Spawn();
                }
            }

        }
    }
    
}
