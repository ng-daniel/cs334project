using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingModule", menuName = "Scriptable Objects/BuildingModule")]
public class BuildingModule : ScriptableObject
{
    [HideInInspector]
    public const float GRID_SIZE = 30.0f;

    public enum ModelType {
        AIR, // Skip this grid cell, but a block/
        SOLID,
        FLOATING,
        TIP
    }

    /// <summary>
    /// How likely choosing this item is according to the height
    /// </summary>
    public enum ChanceType
    {
        CONSTANT,
        POSITIVE_CORRELATION, // Height increases, chance increases
        NEGATIVE_CORRELATION // Height decreases, chance decrease
    }

    public GameObject prefab;
    public ModelType modelType = ModelType.SOLID;
    public ChanceType chanceType = ChanceType.CONSTANT;

    public List<ModelType> CanStackOn; // modules that this type can stack on

    private void Awake()
    {
        BuildStackList();
    }

    /// <summary>
    /// Whether the input module can stack on top of this one
    /// </summary>
    public void BuildStackList()
    {
        CanStackOn = new List<ModelType>();
        switch (this.modelType)
        {
            case ModelType.AIR:
                CanStackOn.Add(ModelType.FLOATING);
                CanStackOn.Add(ModelType.SOLID);
                break;
            case ModelType.SOLID:
                CanStackOn.Add(ModelType.AIR);
                CanStackOn.Add(ModelType.SOLID);
                CanStackOn.Add(ModelType.FLOATING);
                CanStackOn.Add(ModelType.TIP);
                break;
            case ModelType.FLOATING:
                CanStackOn.Add(ModelType.FLOATING);
                CanStackOn.Add(ModelType.AIR);
                CanStackOn.Add(ModelType.SOLID);
                break;
            case ModelType.TIP:
                // Cannot stack anything
                break;
            default:
                Debug.LogError("Invalid building module type.");
                break;
        }
    }

    /// <summary>
    /// Draw the object at this position
    /// </summary>
    /// <param name="pos"></param>
    public void draw(Vector3 pos)
    {
        if (!prefab || this.modelType == ModelType.AIR)
        {
            return;
        }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale = new Vector3(GRID_SIZE, GRID_SIZE, GRID_SIZE);
    }

}
