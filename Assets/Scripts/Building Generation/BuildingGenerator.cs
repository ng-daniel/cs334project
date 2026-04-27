using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using WFC;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;

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
            // If we already specified the slot here, skip
            if (newLevel.slots[i] != null)
            {
                continue;
            }

            int xPos = prevLevel.GetX(i);
            int yPos = prevLevel.GetY(i);

            // Get the module eof slot below ("old") and new slot
            BuildingModule oldModule = prevLevel.slots[i].buildingModule;

            // Assert oldModule is defined and not null
            //Assert.IsNotNull(oldModule);

            BuildingSlot newSlot = new BuildingSlot(chunk, xPos, yPos, newLevel);

            newSlot.buildingModule = GetRandomBuildingModule(oldModule, yPosition);
            newLevel.slots[i] = newSlot;

            // Set the building module of the new slot
            //int newInd = yPos * WFC.Chunk.CHUNK_SIZE + xPos + 1;
            //if (newInd < newLevel.slots.Length && newLevel.slots[newInd] == null) {

            //    if (newSlot.buildingModule.modelType == BuildingModule.ModelType.SOLID)
            //    {
            //        BuildingSlot extraSlot = new BuildingSlot(chunk, xPos + 1, yPos, newLevel);
            //        newLevel.slots[newInd] = extraSlot;
            //    }
            //}

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

    /// <summary>
    /// Get a random building module from the list, according to module chance type and value.
    /// Pass in the y position (height) of the object.
    /// </summary>
    /// <param name="bottomModule"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    private BuildingModule GetRandomBuildingModule(BuildingModule bottomModule, float yPosition)
    {
        List<BuildingModule> buildingModulesList = GenerationManager.instance.buildingModulesList;
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

        // Use weighted probabilities to assign random module
        float total = 0.0f;
        int numBuildingLayers = GenerationManager.instance.numBuildingLayers;
        foreach (BuildingModule module in compatibleModules)
        {
            switch (module.chanceType)
            {
                case BuildingModule.ChanceType.CONSTANT:
                    total += module.chanceValue;
                    break;
                case BuildingModule.ChanceType.POSITIVE_CORRELATION:
                    AnimationCurve positiveCorrelationCurve = GenerationManager.instance.positiveCorrelationCurve;
                    total += module.chanceValue * positiveCorrelationCurve.Evaluate(yPosition / (numBuildingLayers));
                    break;
                case BuildingModule.ChanceType.NEGATIVE_CORRELATION:
                    AnimationCurve negativeCorrelationCurve = GenerationManager.instance.negativeCorrelationCurve;
                    total += module.chanceValue * negativeCorrelationCurve.Evaluate(yPosition / (numBuildingLayers));
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
                if (module.modelType == BuildingModule.ModelType.SOLID)
                {
                    float c = module.chanceValue * GenerationManager.instance.negativeCorrelationCurve.Evaluate(yPosition / (numBuildingLayers * 2));
                    //Debug.Log("Generated " + module.modelType + "at %" + c + "and y pos " + yPosition/(numBuildingLayers*2)); }
                }
                return module;

            }
            else
            {
                randVal -= module.chanceValue;
            }
        }
        return compatibleModules[0];

    }

    /// <summary>
    /// After generating the map, hollow out the blocks that are not seen by setting
    /// the interior flag on each building slot
    /// </summary>
    public void BuildHollowMap()
    {
        // For each level in building map

        foreach (Level<BuildingSlot> level in buildingMap)
        {
            bool[,] visitedSlot = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            // Go through each outer edge that is air, and add to queue

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                BuildingSlot slot1 = level.GetSlot(x, 0);
                BuildingSlot slot2 = level.GetSlot(x, Chunk.CHUNK_SIZE - 1);

                if (slot1.IsEmpty())
                {
                    queue.Enqueue(new Vector2Int(x, 0));
                    visitedSlot[x, 0] = true;
                }

                if (slot2.IsEmpty())
                {
                    queue.Enqueue(new Vector2Int(x, slot2.slotY));
                    visitedSlot[x, slot2.slotY] = true;
                }
            }

            for (int y = 1; y < Chunk.CHUNK_SIZE - 1; y++)
            {
                BuildingSlot slot1 = level.GetSlot(0, y);
                BuildingSlot slot2 = level.GetSlot(Chunk.CHUNK_SIZE - 1, y);

                if (slot1.IsEmpty())
                {
                    queue.Enqueue(new Vector2Int(0, y));
                    visitedSlot[0, y] = true;
                    slot1.isExteriorAir = true;
                }

                if (slot2.IsEmpty())
                {
                    queue.Enqueue(new Vector2Int(slot2.slotX, y));
                    visitedSlot[slot2.slotX, y] = true;
                    slot2.isExteriorAir = true;
                }
            }

            // Go through the queue and mark exterior air

            while (queue.Count > 0)
            {
                Vector2Int slotPos = queue.Dequeue();

                // Check all four directions 

                BuildingSlot[] toCheck = new BuildingSlot[] {
                    level.GetSlot(slotPos.x + 1, slotPos.y),
                    level.GetSlot(slotPos.x - 1, slotPos.y),
                    level.GetSlot(slotPos.x, slotPos.y - 1),
                    level.GetSlot(slotPos.x, slotPos.y + 1)
                };

                for (int i = 0; i < toCheck.Length; i++)
                {
                    BuildingSlot slot = toCheck[i];
                    if (slot == null ||
                        visitedSlot[slot.slotX, slot.slotY])
                    {
                        continue;
                    }

                    if (slot.IsEmpty())
                    {
                        visitedSlot[slot.slotY, slot.slotY] = true;
                        queue.Enqueue(new Vector2Int(slot.slotX, slot.slotY));
                        slot.isExteriorAir = true;
                    }
                }
            }


        } // end for

        // If a block is not empty and touches exterior air, then mark as visible
        //for (int i = 0; i < buildingMap.slots.Length; i++)
        //{

        //}
        // TODO

    }


}
