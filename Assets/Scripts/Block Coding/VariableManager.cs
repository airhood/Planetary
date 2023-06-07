using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    private static List<string> variableNames = new List<string>();
    private static List<Variable> variables = new List<Variable>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int DeclearVariable(string name, DataType dataType, object value)
    {
        variables.Add(new Variable(name, dataType, value));
        variableNames.Add(name);
        return variables.Count - 1;
    }

    public static int VariableNameToAddress(string name)
    {
        try
        {
            return variableNames.IndexOf(name);
        }
        catch
        {
            Log.LogError("VariableManager.VariableNameToAddress: no matching variable name");
            return -1;
        }
    }

    public static object AccessVariable_Get(int address)
    {
        try
        {
            return variables[address].value;
        }
        catch
        {
            Log.LogError("VariableManager.AccessVariable_Get: error accessing variable");
            return null;
        }
    }

    public static void AccessVariable_Set(int address, object value)
    {
        try
        {
            variables[address].value = value;
        }
        catch
        {
            Log.LogError("VariableManager.AccessVariable_Get: error accessing variable");
        }
    }

    public static DataType AccessVariable_GetDataType(int address)
    {
        return variables[address].dataType;
    }
}
