using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    print,
    Start, OnActiveTrue, OnActiveFalse, Tick, OnEvent,
    _if, _if_else, _for, _while, _switch, _try_catch,
    ADD, SUBSTRACT, MULTIPLY, DIVIDE, Pow, Sqrt, Abs, Vector2_Distance, Vector2_magnitude,
    Combine, Split, toChar, toString,
    EqualOperator, NotEqualOperater, LeftBigger, RightBigger, LeftBiggerOrSame, RightBiggerOrSame, ANDOperator, OROperator,
    setActive, getActiveState, getPortData, setPortData,
    Time_day, Time_hour, Time_minute,
    sendDataAt, readSignalFrom, getSignalOrigin
}

public enum DataType
{
    _int, _float, _bool, _string, _Vector2, _Vector2Int, NULL, VOID, Node
}

public struct NodeParameter
{
    public DataType dataType;
    public Type type;
    public object value;

    public NodeParameter(DataType dataType, object value)
    {
        type = value.GetType();
        switch(dataType)
        {
            case DataType._int:
                if (type != typeof(int?))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType._float:
                if (type != typeof(float?))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType._bool:
                if (type != typeof(bool?))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType._string:
                if (type != typeof(string))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType._Vector2:
                if (type != typeof(Vector2?))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType._Vector2Int:
                if (type != typeof(Vector2Int?))
                {
                    Log.LogError("DataType not matching");
                }
                break;
            case DataType.Node:
                if (type != typeof(Node))
                {
                    Log.LogError("DataType not matching");
                }
                break;
        }
        this.dataType = dataType;
        this.value = value;
    }
}

[System.Serializable]
public class Node
{
    public NodeType type;
    public bool hasNodeBracket;
    public List<NodeParameter> parameters;
    public DataType returnType;
    public List<NodeBracket>? nodeBrackets;

    public Node(NodeType type, List<NodeParameter> parameters, DataType returnType)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets = new List<NodeBracket>();
        hasNodeBracket = false;
        this.returnType = returnType;
    }

    public Node(NodeType type, List<NodeParameter> parameters, Node nodeInBracket, DataType returnType)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets.Add(new NodeBracket(nodeInBracket));
        hasNodeBracket = true;
        this.returnType = returnType;
    }

    public Node(NodeType type, List<NodeParameter> parameters, NodeBracket nodeBracket, DataType returnType)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets = new List<NodeBracket>();
        nodeBrackets.Add(nodeBracket);
        hasNodeBracket = true;
        this.returnType = returnType;
    }

    public Node(NodeType type, List<NodeParameter> parameters, List<NodeBracket> nodeBrackets, DataType returnType)
    {
        this.parameters = parameters;
        this.type = type;
        this.nodeBrackets = nodeBrackets;
        hasNodeBracket = true;
        this.nodeBrackets = nodeBrackets;
        this.returnType = returnType;
    }
}

[System.Serializable]
public class NodeBracket
{
    public string bracketName;
    public bool nameVisible;
    public List<Node> nodes;

    public NodeBracket()
    {
        bracketName = null;
        nameVisible = false;
        nodes = new List<Node>();
    }

    public NodeBracket(string bracketName)
    {
        this.bracketName = bracketName;
        nameVisible = true;
        nodes = new List<Node>();
    }

    public NodeBracket(Node node)
    {
        bracketName = null;
        nameVisible = false;
        nodes.Add(node);
    }

    public NodeBracket(string bracketName, bool nameVisible)
    {
        this.bracketName = bracketName;
        this.nameVisible = nameVisible;
        nodes = new List<Node>();
    }

    public NodeBracket(string bracketName, Node node)
    {
        this.bracketName = bracketName;
        nameVisible = true;
        nodes.Add(node);
    }

    public NodeBracket(string bracketName, bool nameVisible, Node node)
    {
        this.bracketName = bracketName;
        this.nameVisible = nameVisible;
        nodes.Add(node);
    }
}