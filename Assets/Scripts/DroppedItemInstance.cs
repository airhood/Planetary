using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroppedItemData
{
    public ItemStack itemStack;

    public DroppedItemData(short itemID, int amount)
    {
        itemStack = new ItemStack(itemID, amount);
    }
}

public class DroppedItemInstance : MonoBehaviour
{
    public Player player;

    public int entityID;
    public int droppedItemDataID;
    public int spawnedItemGameObjectID;
    public ItemStack itemStack;
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
