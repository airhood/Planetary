using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemInstance : MonoBehaviour
{
    public Player player;

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
