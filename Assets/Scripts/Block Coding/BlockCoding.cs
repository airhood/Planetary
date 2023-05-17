using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCoding : MonoBehaviour
{
    public List<GameObject> NodePanelMenus;
    
    public List<Node> nodes;

    // Start is called before the first frame update
    void Start()
    {
        NodePanelMenus = new List<GameObject>();
        nodes = new List<Node>();
        Node node = new Node(NodeType.Tick, null);
        NodeBracket nb = new NodeBracket();
        nb.nodes.Add(node);
        nodes.Add(new Node(NodeType.Start, null, nb));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewNodeTree(Node node)
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
