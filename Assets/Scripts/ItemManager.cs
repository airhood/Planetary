using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Main main;

    public GameObject itemPrefab;
    public GameObject item;

    public Player player;

    public void spawnItems(short itemID, Vector2 pos, ushort eachAmount, byte itemGroupAmount)
    {
        for(int i = 0; i < itemGroupAmount; i++)
        {
            spawnItem(itemID, pos, eachAmount);
        }
    }

    public void spawnItem(short itemID, Vector2 pos, ushort amount)
    {
        print($"itemID: {itemID}");
        int relativeID = main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData.Count;
        main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData.Add(new DroppedItemData(itemID, amount));
        int entityID = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        main.world.planet[0].map.entitySystem.visibleEntities.Add(new EntityData(pos, EntityType.DroppedItem, relativeID));
        GameObject gameObject = Instantiate(itemPrefab, pos, Quaternion.identity);
        gameObject.transform.SetParent(item.transform);
        gameObject.GetComponent<SpriteRenderer>().sprite = Main.itemList[itemID].image;
        DroppedItemInstance droppedItemInstance = gameObject.AddComponent<DroppedItemInstance>();
        droppedItemInstance.entityID = entityID;
        droppedItemInstance.droppedItemDataID = relativeID;
        droppedItemInstance.itemID = itemID;
        droppedItemInstance.amount = amount;
        droppedItemInstance.collectTickLeft = -1;
        droppedItemInstance.isBeingCollected = false;
        droppedItemInstance.player = player;
        //gameObject.AddComponent<Rigidbody2D>();
        gameObject.layer = 10;
        gameObject.tag = "dropped_item";
        droppedItemInstance.spawnedItemGameObjectID = main.world.planet[0].map.entitySystem.spawnedEntityGameObject.Count;
        main.world.planet[0].map.entitySystem.spawnedEntityGameObject.Add(gameObject);
    }

    public void respawnItem(int entityID, EntityData entityData)
    {
        DroppedItemData droppedItemData = main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData[entityData.relatedID];
        GameObject gameObject = Instantiate(itemPrefab, entityData.position, Quaternion.identity);
        gameObject.transform.SetParent(item.transform);
        gameObject.GetComponent<SpriteRenderer>().sprite = Main.itemList[droppedItemData.itemID].image;
        DroppedItemInstance droppedItemInstance = gameObject.AddComponent<DroppedItemInstance>();
        droppedItemInstance.entityID = entityID;
        droppedItemInstance.droppedItemDataID = entityData.relatedID;
        droppedItemInstance.itemID = droppedItemData.itemID;
        droppedItemInstance.amount = droppedItemData.amount;
        droppedItemInstance.collectTickLeft = -1;
        droppedItemInstance.isBeingCollected = false;
        droppedItemInstance.player = player;
        //gameObject.AddComponent<Rigidbody2D>();
        gameObject.layer = 10;
        gameObject.tag = "dropped_item";
        main.world.planet[0].map.entitySystem.spawnedEntityGameObject.Add(gameObject);
        droppedItemInstance.spawnedItemGameObjectID = main.world.planet[0].map.entitySystem.spawnedEntityGameObject.Count;
        main.world.planet[0].map.entitySystem.spawnedEntityGameObject.Add(gameObject);
    }

    public void updateEntityRelatedID()
    {
        int count = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        int k = 0;
        for(int i = 0; i < count; i++)
        {
            if (main.world.planet[0].map.entitySystem.visibleEntities[i].type == EntityType.DroppedItem)
            {
                main.world.planet[0].map.entitySystem.visibleEntities[i].relatedID = k;
                k++;
            }
        }
    }

    public void updateDroppedItemInstance(int removedIndex)
    {
        int count = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        bool update = false;
        for(int i = 0; i < count; i++)
        {
            if (i == removedIndex)
            {
                update = true;
            }
            if (update)
            {
                var droppedItemInstance = main.world.planet[0].map.entitySystem.spawnedEntityGameObject[i].GetComponent<DroppedItemInstance>();
                droppedItemInstance.entityID--;
                droppedItemInstance.droppedItemDataID--;
                droppedItemInstance.spawnedItemGameObjectID--;
            }
        }
    }
}
