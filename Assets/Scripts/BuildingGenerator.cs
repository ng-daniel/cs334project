using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WFC;

public class BuildingGenerator : MonoBehaviour
{
    private readonly Chunk chunk;

    public List<Level> buildingMap;
    public BuildingGenerator(Chunk chunk)
    {
        this.chunk = chunk;
        this.buildingMap = new List<Level>();
    }

    public void Start()
    {

        // Randomly fill the building map


        DebugDraw();
    }

    Slot GetSlotAtPos(int x, int y, int levelInd)
    {
        Level level = buildingMap[levelInd];
        if (level == null)
        {
            return null;
        }

        
    }

    void DebugDraw()
    {
        foreach (Level level in buildingMap)
        {
            foreach (Slot slot in level.slots)
            {
                Vector3 pos = WorldPos()
            }
        }
    }
    
}
