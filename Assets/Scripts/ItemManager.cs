using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject item;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnItems(short itemID, Vector2 pos, ushort eachAmount, byte itemGroupAmount)
    {
        for(int i = 0; i < itemGroupAmount; i++)
        {
            spawnItem(itemID, pos, eachAmount);
        }
    }

    public void spawnItem(short itemID, Vector2 pos, ushort amount)
    {
        GameObject gameObject = Instantiate(itemPrefab, pos, Quaternion.identity);
        gameObject.transform.SetParent(item.transform);
        gameObject.GetComponent<SpriteRenderer>().sprite = Main.itemList[itemID].image;
        DroppedItemInstance droppedItemInstance = gameObject.AddComponent<DroppedItemInstance>();
        droppedItemInstance.itemID = itemID;
        droppedItemInstance.amount = amount;
        droppedItemInstance.collectTickLeft = -1;
        droppedItemInstance.isBeingCollected = false;
        droppedItemInstance.player = player;
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.layer = 10;
        gameObject.tag = "dropped_item";
    }
}
