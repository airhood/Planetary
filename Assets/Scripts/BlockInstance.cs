using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Block
{
    public Vector2Int position;
    public Rotation rotation;
    public string state;
    public List<MatterStore> internalStorage = new List<MatterStore>();

    public Door(Vector2Int position, Rotation rotation, string state)
    {
        this.position = position;
        this.rotation = rotation;
        this.state = state;
    }
}

public class MatterStore
{
    public Matter matter;
    public float amount;
}

public class BlockInstance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddBlockInstance(Vector2Int position, Block block)
    {

    }

    public void RemoveBlockInstance(Vector2Int position, Block block)
    {

    }

    public T RunBockInstanceFunction<T>(short blockCode, string command)
    {
        T returnValue = default(T);

        switch(blockCode)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                Door(command);
                break;
        }

        return returnValue;
    }

    private bool Door(string command)
    {
        switch(command)
        {
            case "changeDoorOpenState":
                break;
        }
        return false;
    }
}
