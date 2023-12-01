using UnityEngine;
using System;

[System.Serializable]
public class SystemType
{
    public enum TypeEnum
    {
        Float,
        Integer,
        String,
        Boolean,
        Vector2,
        Vector3,
    }

    public static Type GetTypeFromEnum(TypeEnum typeEnum)
    {
        switch (typeEnum)
        {
            case TypeEnum.Float:
                return typeof(float);
            case TypeEnum.Integer:
                return typeof(int);
            case TypeEnum.String:
                return typeof(string);
            case TypeEnum.Boolean:
                return typeof(bool);
            case TypeEnum.Vector2:
                return typeof(Vector2);
            case TypeEnum.Vector3:
                return typeof(Vector3);
            default:
                return null;
        }
    }
}