using UnityEngine;
using WFC;
using static BuildingModule;

public class BuildingSlot : Slot
{

    public BuildingModule buildingModule;
    private GameObject instantiatedPrefab = null;

    public BuildingSlot(Chunk chunk, int slotX, int slotY) : base(chunk, slotX, slotY)
    {
        buildingModule = null;
    }

    /// <summary>
    /// Draw the object at this position
    /// </summary>
    /// <param name="pos"></param>
    public override void Spawn()
    {
        if (!buildingModule ||
            !buildingModule.prefab || 
            buildingModule.modelType == ModelType.EMPTY)
        {
            return;
        }

        Vector3 pos = WorldPos();

        if (this.instantiatedPrefab)
        {
            GameObject.Destroy(this.instantiatedPrefab);
        }

        this.instantiatedPrefab = GameObject.Instantiate(buildingModule.prefab, pos, Quaternion.identity);
        this.instantiatedPrefab.transform.localScale *= SLOT_SIZE;
    }


}
