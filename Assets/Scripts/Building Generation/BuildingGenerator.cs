using NUnit.Framework;
using System.Collections.Generic;
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
    public void AddLevel(float levelHeight)
    {

        Level<BuildingSlot> level = new Level<BuildingSlot>(chunk, Chunk.CHUNK_SIZE, levelHeight);
        List<BuildingModule> modulesList = GenerationManager.instance.buildingModulesList;

        if (modulesList == null || modulesList.Count == 0)
        {
            Debug.Log("Generation Manager's Building Modules List is empty.");
            return;
        }

        // temp - randomly fill the building map
        for (int i = 0; i < level.slots.Length; i++)
        {
            level.slots[i] = new BuildingSlot(chunk, level.GetX(i), level.GetY(i));

            if (Random.Range(0, 10) < 2)
            {
                // Assign random module
                int randInd = Random.Range(0, modulesList.Count);
                level.slots[i].buildingModule = modulesList[randInd];
            } else
            {
                level.slots[i].buildingModule = modulesList[0];
            }
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
