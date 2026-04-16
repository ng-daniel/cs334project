using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using WFC;

[CreateAssetMenu(fileName = "BuildingModule", menuName = "Scriptable Objects/BuildingModule")]
public class BuildingModule : ScriptableObject
{
    public enum ModelType {
        EMPTY, // Skip this grid cell, but a block/
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

    /// <summary>
    /// Whether this module can stop on the input module below
    /// </summary>
    public bool CanStackOnType(ModelType below)
    {
        switch (this.modelType)
        {
            case ModelType.EMPTY:
                return below == ModelType.FLOATING ||
                       below == ModelType.EMPTY ||
                       below == ModelType.SOLID;
            case ModelType.SOLID:
                return true;
            case ModelType.FLOATING:
                return below == ModelType.FLOATING || 
                       below == ModelType.EMPTY || 
                       below == ModelType.SOLID;
            case ModelType.TIP:
                return below == ModelType.SOLID;
            default:
                Debug.LogError("Invalid building module type.");
                return false;
        }
    }

}
