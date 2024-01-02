using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StorageContentType
{
    matter, item
}

public struct StorageContent
{
    public StorageContentType type;
    public object content;
}

public class BlockInstance
{
    public string block_name { get; private set; }
    public short blockID { get; private set; }
    public Vector2Int position { get; set; }
    public Rotation rotation { get; set; }
    public byte state { get; set; }
    public List<StorageContent> storageContents { get; set; }

    public BlockInstance(short blockID, Vector2Int position, Rotation rotation, byte state)
    {
        this.block_name = Main.data.blockList[blockID].name;
        this.blockID = blockID;
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
    public List<BlockInstance> blockInstances = new List<BlockInstance>();

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
        BlockInstance instance;
        switch(blockID)
        {
            case 2:
                instance = new BlockInstance(blockID, position, rotation, 0);
                break;
            default:
                return;
        }
        positions.Add(position);
        blockInstances.Add(instance);
    }

    public BlockInstance GetBlockInstance(Vector2Int position)
    {
        try
        {
            return blockInstances[positions.IndexOf(position)];
        }
        catch (Exception ex)
        {
            Log.LogError($"BlockInstanceManager.GetBlockInstance: Cannot get block instance. Position: {position}. Error Code: {ex}");
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
            InvokeBlockFunc(blockInstances[i], "tick", null);
        }
    }

    public void InteractBlock(Vector2Int position)
    {
        InvokeBlockFunc(blockInstances[positions.IndexOf(position)], "interact", null);
    }

    public static void InvokeBlockFunc(BlockInstance blockInstance, string functionTag, object[] parameters)
    {
        string methodName = blockInstance.block_name + "__" + functionTag;
        if (!typeof(BlockInstanceFunction).GetMethod(methodName).IsStatic)
        {
            Log.LogError("BlockInstanceManager.InvokeBlockFunc: BlockInstanceFunction isn't static");
            return;
        }
        object[] _parameters = new object[parameters.Length + 1];
        _parameters[0] = blockInstance;
        try
        {
            for(int i = 1; i < _parameters.Length; i++)
            {
                _parameters[i] = parameters[i - 1];
            }
        } catch (Exception ex)
        {
            Log.LogError($"BlockInstanceManager.InvokeBlockFunc: Error grouping function parameters. Error code: {ex}");
            return;
        }
        typeof(BlockInstanceFunction).GetMethod(methodName).Invoke(null, _parameters);
    }
}
