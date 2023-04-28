using System;
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

public class WaterElectrolysis : IBlockInstance
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
        Debug.Log("Interaction");
    }

    public WaterElectrolysis(Vector2Int position, Rotation rotation, byte state)
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
    public List<Vector2Int> positions = new List<Vector2Int>();
    public List<IBlockInstance> blockInstances = new List<IBlockInstance>();

    int tick;
    public int blockInstanceTickRate;

    public void BlockInstanceTick()
    {
        tick++;
        if (tick == blockInstanceTickRate)
        {
            tick = 0;
            CalculateInstances();
        }
    }

    public void AddBlockInstance(Vector2Int position, short blockID, Rotation rotation)
    {
        object instance;
        switch(blockID)
        {
            case 2:
                instance = new WaterElectrolysis(position, rotation, 0);
                break;
            default:
                return;
        }
        positions.Add(position);
        blockInstances.Add((IBlockInstance)instance);
    }

    public IBlockInstance? GetBlockInstance(Vector2Int position)
    {
        try
        {
            return blockInstances[positions.IndexOf(position)];
        }
        catch (Exception ex)
        {
            Debug.LogError($"BlockInstanceManager.GetBlockInstance: Cannot get block instance. Position: {position}. Error Code: {ex}");
        }

        return null;
    }

    public void RemoveBlockInstance(Vector2Int position)
    {
        int index = positions.IndexOf(position);
        positions.RemoveAt(index);
        blockInstances.RemoveAt(index);
    }

    public void CalculateInstances()
    {
        int count = blockInstances.Count;
        for(int i = 0; i < count; i++)
        {
            blockInstances[i].InstanceTick();
        }
    }

    public void InteractBlock(Vector2Int position)
    {
        blockInstances[positions.IndexOf(position)].Interaction();
    }
}
