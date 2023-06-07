using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Variable
{
    public string name;
    public DataType dataType;
    public Type type;
    public object value;
    public bool isNull;

    public Variable(string name, DataType dataType, object value)
    {
        this.name = name;
        this.dataType = dataType;
        this.value = value;
        isNull = (value == null);
        switch (dataType)
        {
            case DataType._int:
                type = typeof(int?);
                break;
            case DataType._float:
                type = typeof(float?);
                break;
            case DataType._bool:
                type = typeof(bool?);
                break;
            case DataType._string:
                type = typeof(string);
                break;
            case DataType._Vector2:
                type = typeof(Vector2?);
                break;
            case DataType._Vector2Int:
                type = typeof(Vector2Int?);
                break;
            default:
                type = null;
                Log.LogError($"Variable: dataType {dataType} is not supported.");
                break;
        }
    }
}

public class vList
{
    public string name;
    public DataType dataType;
    public Type type;
    public ArrayList value;

    public vList(string name, DataType dataType)
    {
        this.name = name;
        this.dataType = dataType;
        switch (dataType)
        {
            case DataType._int:
                type = typeof(int?);
                break;
            case DataType._float:
                type = typeof(float?);
                break;
            case DataType._bool:
                type = typeof(bool?);
                break;
            case DataType._string:
                type = typeof(string);
                break;
            case DataType._Vector2:
                type = typeof(Vector2?);
                break;
            case DataType._Vector2Int:
                type = typeof(Vector2Int?);
                break;
            default:
                type = null;
                Log.LogError($"Variable: dataType {dataType} is not supported.");
                break;
        }
        value = new ArrayList();
    }
}