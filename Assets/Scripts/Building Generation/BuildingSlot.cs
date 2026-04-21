using UnityEngine;
using WFC;
using static BuildingModule;

public class BuildingSlot : Slot
{
    public readonly Level<BuildingSlot> levelReference;

    public BuildingModule buildingModule;
    public GameObject instantiatedPrefab = null;

    public BuildingSlot(Chunk chunk, int slotX, int slotY, Level<BuildingSlot> levelReference) : base(chunk, slotX, slotY)
    {
        buildingModule = null;
        this.levelReference = levelReference;
    }

    /// <summary>
    /// Draw the object at this position
    /// </summary>
    /// <param name="pos"></param>
    public override void Spawn()
    {
        if (this.instantiatedPrefab)
        {
            return;
        }

        if (!buildingModule ||
            !buildingModule.prefab || 
            buildingModule.modelType == ModelType.EMPTY)
        {
            return;
        }

        Vector3 pos = WorldPos();
        pos.y = levelReference.levelYPosition;

        this.instantiatedPrefab = GameObject.Instantiate(buildingModule.prefab, pos, buildingModule.prefab.transform.rotation);
        this.instantiatedPrefab.transform.localScale *= 2;
    }


}
