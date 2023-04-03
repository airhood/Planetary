using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IBlockInstance
{
    Vector2Int position { get; set; }
    Rotation rotation { get; set; }
    byte state { get; set; }
    List<StorageContent> storageContents { get; set; }
    void InstanceTick();
    void Interaction();
}

public enum StorageContentType
{
    matter, item
}

public struct StorageContent
{
    public StorageContentType type;
    public object content;
}

public class Door : IBlockInstance
{
    public Vector2Int position { get; set; }
    public Rotation rotation { get; set; }
    public byte state { get; set; }
    public List<StorageContent> storageContents { get; set; }

    public void InstanceTick()
    {
        
    }

    public void Interaction()
    {

    }

    public Door(Vector2Int position, Rotation rotation, byte state)
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

public class BlockInstanceManager : MonoBehaviour
{
    public List<IBlockInstance> blockInstances = new List<IBlockInstance>();

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

    public void RunInstances()
    {

    }
}
