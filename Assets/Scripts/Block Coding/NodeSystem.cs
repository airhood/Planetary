using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    Start, OnActiveTrue, OnActiveFalse, Tick, OnEvent
}

public class NodeSystem : MonoBehaviour
{
    public List<GameObject> NodePanelMenus;
    
    public static List<Node> nodes;

    // Start is called before the first frame update
    void Start()
    {
        NodePanelMenus = new List<GameObject>();
        nodes = new List<Node>();
        NodeBracket nb = new NodeBracket();
        List<NodeParameter> nodeParameters = new List<NodeParameter>();
        nb.nodes.Add(new Node(NodeType.print, nodeParameters, DataType.VOID));
        nodeParameters.Add(new NodeParameter(DataType._string, "wow"));
        nodes.Add(new Node(NodeType.Tick, null, nb, DataType.VOID));

        //EvokeEvent(EventType.Tick, null);

        NodeCommand.DECLEAR_VARIABLE("var", DataType._string);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void NodeTick()
    {

    }

    public static NodeType? EventTypeToNodeType(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Start:
                return NodeType.Start;
            case EventType.OnActiveTrue:
                return NodeType.OnActiveTrue;
            case EventType.OnActiveFalse:
                return NodeType.OnActiveFalse;
            case EventType.Tick:
                return NodeType.Tick;
            case EventType.OnEvent:
                return NodeType.OnEvent;
            default:
                return null;
        }
    }

    public static void EvokeEvent(EventType eventType, string content)
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].type == EventTypeToNodeType(eventType))
            {
                RunNodeBracket(nodes[i].nodeBrackets[0]);
            }
        }
    }

    public static object RunNode(Node node)
    {
        if (node == null) Log.LogError("NodeSystem.RunNode: cannot run node. null exception");
        switch(node.type)
        {
            case NodeType.print:
                NodeCommand.PRINT(node.parameters[0].value);
                return null;
            case NodeType._if:
                if (!node.hasNodeBracket) return null;
                Node n = node.parameters[0].value as Node;
                if (n != null)
                {
                    bool? b = RunNode(n) as bool?;
                    if (b != null)
                    {
                        NodeCommand.IF(node, (bool)b);
                    }
                }
                return null;
            case NodeType._if_else:
                return null;
            case NodeType._for:
                return null;
            case NodeType._while:
                return null;
            case NodeType._switch:
                return null;
            case NodeType._try_catch:
                return null;
            case NodeType.ADD:
                return null;
            case NodeType.SUBSTRACT:
                return null;
            case NodeType.MULTIPLY:
                return null;
            case NodeType.DIVIDE:
                return null;
            case NodeType.Pow:
                return null;
            case NodeType.Sqrt:
                return null;
            case NodeType.Abs:
                return null;
            case NodeType.Vector2_Distance:
                return null;
            case NodeType.Vector2_magnitude:
                return null;
            case NodeType.Combine:
                return null;
            case NodeType.Split:
                return null;
            case NodeType.toChar:
                return null;
            case NodeType.toString:
                return null;
            case NodeType.EqualOperator:
                return null;
            case NodeType.NotEqualOperater:
                return null;
            case NodeType.LeftBigger:
                return null;
            case NodeType.RightBigger:
                return null;
            case NodeType.LeftBiggerOrSame:
                return null;
            case NodeType.RightBiggerOrSame:
                return null;
            case NodeType.ANDOperator:
                return null;
            case NodeType.OROperator:
                return null;
            default:
                Log.LogError("NodeSystem.RunNode: node type not found");
                return null;
        }
    }

    public static void RunNodeBracket(NodeBracket nb)
    {
        for(int i = 0; i < nb.nodes.Count; i++)
        {
            RunNode(nb.nodes[i]);
        }
    }

    public static void CreateNewNodeTree(NodeType type, Node node)
    {
        nodes.Add(node);
    }

    public void NodePanelUI(int menuNum)
    {
        for(int i = 0; i < NodePanelMenus.Count; i++)
        {
            if (i == menuNum)
            {
                NodePanelMenus[i].SetActive(true);
                continue;
            }
            NodePanelMenus[i].SetActive(false);
        }
    }
}
