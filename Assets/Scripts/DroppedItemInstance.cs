using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroppedItemData
{
    public short itemID;
    public ushort amount;

    public DroppedItemData(short itemID, ushort amount)
    {
        this.itemID = itemID;
        this.amount = amount;
    }
}

public class DroppedItemInstance : MonoBehaviour
{
    public Player player;

    public int entityID;
    public int droppedItemDataID;
    public int spawnedItemGameObjectID;
    public short itemID;
    public ushort amount;
    public short collectTickLeft;
    public bool isBeingCollected;

    void Update()
    {
        if (isBeingCollected)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);
            if (distance > player.itemCollectDistance) isBeingCollected = false;
        }
    }
}
