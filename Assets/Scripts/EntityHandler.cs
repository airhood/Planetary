using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHandler : MonoBehaviour
{
    public Player player;
    public Main main;
    public ItemManager itemManager;
    public float despawnDistance;
    public float respawnDistance;

    int tick;
    public int droppedItemCheckTickRate;

    public void EntityTick()
    {
        tick++;
        if (tick == droppedItemCheckTickRate)
        {
            tick = 0;
            DroppedItemCheckTick();
            print($"player.transform.position:{player.transform.position}");
        }
    }

    public void DroppedItemCheckTick()
    {
        DespawnCheck();
        RespawnCheck();
        UpdateEntityPosition();
    }

    public void RespawnCheck()
    {
        int amount = main.world.planet[0].map.entitySystem.hiddenEntities.Count;
        for(int i = 0; i < amount; i++)
        {
            if (Vector2.Distance(player.transform.position, main.world.planet[0].map.entitySystem.hiddenEntities[i].position) <= respawnDistance)
            {
                var entityData = main.world.planet[0].map.entitySystem.hiddenEntities[i];
                switch (main.world.planet[0].map.entitySystem.hiddenEntities[i].type)
                {
                    case EntityType.DroppedItem:
                        itemManager.respawnItem(i, main.world.planet[0].map.entitySystem.hiddenEntities[i]);
                        main.world.planet[0].map.entitySystem.hiddenEntities.RemoveAt(i);
                        main.world.planet[0].map.entitySystem.visibleEntities.Add(entityData);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DespawnCheck()
    {
        int amount = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        for(int i = 0; i < amount; i++)
        {
            if (Vector2.Distance(player.transform.position, main.world.planet[0].map.entitySystem.visibleEntities[i].position) >= despawnDistance)
            {
                var entityData = main.world.planet[0].map.entitySystem.visibleEntities[i];
                main.world.planet[0].map.entitySystem.visibleEntities.RemoveAt(i);
                main.world.planet[0].map.entitySystem.hiddenEntities.Add(entityData);
                Destroy(main.world.planet[0].map.entitySystem.spawnedEntityGameObject[i]);
                main.world.planet[0].map.entitySystem.spawnedEntityGameObject.RemoveAt(i);
            }
        }
    }

    public void UpdateEntityPosition()
    {
        int amount = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        for(int i = 0; i < amount; i++)
        {
            var entity = main.world.planet[0].map.entitySystem.visibleEntities[i];
            entity.position = main.world.planet[0].map.entitySystem.spawnedEntityGameObject[entity.relatedID].transform.position;
        }
        print("updated entity position");
    }
}
