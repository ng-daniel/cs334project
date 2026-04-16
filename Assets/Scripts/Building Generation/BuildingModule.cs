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
        switch (below)
        {
            // below empty, this can be empty TODO add floating very low chance
            case ModelType.EMPTY:
                return this.modelType == ModelType.EMPTY;
            // Anything can stack on solid
            case ModelType.SOLID:
                return this.modelType == ModelType.SOLID || 
                       this.modelType == ModelType.EMPTY ||
                       this.modelType == ModelType.TIP ||
                       this.modelType == ModelType.FLOATING;
            // Air/floating can stack on floating
            case ModelType.FLOATING:
                return this.modelType == ModelType.EMPTY ||
                       this.modelType == ModelType.FLOATING ||
                       this.modelType == ModelType.SOLID;
            // Empty can stack on tip
            case ModelType.TIP:
                return this.modelType == ModelType.EMPTY;
            default:
                Debug.LogError("Model type is not valid.");
                return false;
        }
    }

}
