using UnityEngine;
using WFC;
using static BuildingModule;

public class BuildingSlot : Slot
{

    public BuildingModule buildingModule;
    public GameObject instantiatedPrefab = null;

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
        //pos.x *= 2;
        //pos.z *= 2;

        this.instantiatedPrefab = GameObject.Instantiate(buildingModule.prefab, pos, buildingModule.prefab.transform.rotation);
        this.instantiatedPrefab.transform.localScale *= 2;
    }


}
