using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using WFC;

[CreateAssetMenu(fileName = "BuildingModule", menuName = "Scriptable Objects/BuildingModule")]
public class BuildingModule : ScriptableObject
{
    public int height = 1; // 1 to 4
    public enum ModelType {
        EMPTY, // Skip this grid cell, but a block/
        SOLID,
        FLOATING,
        TIP,
        SOLID2
    }

    /// <summary>
    /// How likely choosing this item is according to the height
    /// </summary>
    public enum ChanceType
    {
        CONSTANT,
        POSITIVE_CORRELATION,
        NEGATIVE_CORRELATION
    }

    public GameObject prefab;
    public ModelType modelType = ModelType.SOLID;

    /// <summary>
    /// Chance type constant: chanceValue is constant
    /// Chance type Gaussian: chanceValue is the mean height position, std dev 1
    /// </summary>
    public ChanceType chanceType = ChanceType.CONSTANT;

    public float chanceValue = 0.5f;

    /// <summary>
    /// Whether this module can stop on the input module below
    /// </summary>
    public bool CanStackOnType(ModelType below)
    {
        switch (below)
        {
            // Empty and floating stacks on empty
            case ModelType.EMPTY:
                return this.modelType == ModelType.EMPTY ||
                       this.modelType == ModelType.FLOATING;
            // Anything can stack on solid
            case ModelType.SOLID:
                return this.modelType == ModelType.SOLID || 
                       this.modelType == ModelType.SOLID2 ||
                       this.modelType == ModelType.EMPTY ||
                       this.modelType == ModelType.TIP ||
                       this.modelType == ModelType.FLOATING;
            case ModelType.SOLID2:
                return this.modelType == ModelType.SOLID2 ||
                       this.modelType == ModelType.EMPTY;
            // Air/floating can stack on floating
            case ModelType.FLOATING:
                return this.modelType == ModelType.EMPTY ||
                       this.modelType == ModelType.FLOATING;
            // Empty or floating can stack on tip
            case ModelType.TIP:
                return this.modelType == ModelType.EMPTY;
            default:
                Debug.LogError("Model type is not valid.");
                return false;
        }
    }

}
