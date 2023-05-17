using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Start, OnActiveTrue, OnActiveFalse, Tick, OnEvent
}

public enum DataType
{
    _int, _float, _bool, _string, _Vector2, _Vector2Int
}

[System.Serializable]
public struct NodeParameter<T>
{
    public DataType dataType;
    public T value;

    public NodeParameter(DataType dataType, T value)
    {
        switch(dataType)
        {
            case DataType._int:
                if (typeof(T) != typeof(int?))
                {
                    Debug.LogError("DataType not matching");
                }
                break;
            case DataType._float:
                if (typeof(T) != typeof(float?))
                {
                    Debug.LogError("DataType not matching");
                }
                break;
            case DataType._bool:
                if (typeof(T) != typeof(bool?))
                {
                    Debug.LogError("DataType not matching");
                }
                break;
            case DataType._string:
                if (typeof(T) != typeof(string))
                {
                    Debug.LogError("DataType not matching");
                }
                break;
            case DataType._Vector2:
                if (typeof(T) != typeof(Vector2?))
                {
                    Debug.LogError("DataType not matching");
                }
                break;
            case DataType._Vector2Int:
                if (typeof(T) != typeof(Vector2Int?))
                {
                    Debug.LogError("DataType not matching");
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
    public bool hasNextNode;
    public ArrayList parameters;
    public List<NodeBracket>? nodeBrackets;

    public Node(NodeType type, ArrayList parameters)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets = new List<NodeBracket>();
        hasNextNode = false;
    }

    public Node(NodeType type, ArrayList parameters, Node nodeInBracket)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets.Add(new NodeBracket(nodeInBracket));
        hasNextNode = true;
    }

    public Node(NodeType type, ArrayList parameters, NodeBracket nodeBracket)
    {
        this.parameters = parameters;
        this.type = type;
        nodeBrackets = new List<NodeBracket>();
        nodeBrackets.Add(nodeBracket);
        hasNextNode = true;
        nodeBrackets.Add(nodeBracket);
    }

    public Node(NodeType type, ArrayList parameters, List<NodeBracket> nodeBrackets)
    {
        this.parameters = parameters;
        this.type = type;
        this.nodeBrackets = nodeBrackets;
        hasNextNode = true;
        this.nodeBrackets = nodeBrackets;
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