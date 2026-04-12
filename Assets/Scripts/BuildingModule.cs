using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingModule", menuName = "Scriptable Objects/BuildingModule")]
public class BuildingModule : ScriptableObject
{
    public enum ModelType {
        AIR,
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

    public ModelType modelType;

    /// <summary>
    /// Whether the input module can stack on top of this one
    /// </summary>
    public bool CanStack(BuildingModule other)
    {
        switch (this.type)
        {
            case ModelType.AIR:
                return (other.modelType == ModelType.FLOATING);
            case ModelType.SOLID:
                return (other.modelType == ModelType.SOLID) ||
                       (other.modelType == ModelType.AIR) ||
                       (other.modelType == ModelType.;
                break;
            case ModelType.FLOATING:
                break;
            case ModelType.TIP:
                break;
            default:
                Debug.LogError("Invalid building module type.");
                return false;

        }
    }

    /// <summary>
    /// Draw the object at this position
    /// </summary>
    /// <param name="pos"></param>
    public void draw(Vector3 pos)
    {

    }

}
