using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroppedItemData : EntityData
{
    public Vector2 position { get; set; }
    public short itemID;
    public ushort amount;

    public DroppedItemData(Vector2 position, short itemID, ushort amount)
    {
        this.position = position;
        this.itemID = itemID;
        this.amount = amount;
    }
}

public class DroppedItemInstance : MonoBehaviour
{
    public Player player;

    public int entityID;
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
