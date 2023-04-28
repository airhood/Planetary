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
        }
    }

    public void DroppedItemCheckTick()
    {
        print("DroppedItemCheckTick");
        DespawnCheck();
        RespawnCheck();
    }

    public void RespawnCheck()
    {
        print("respawn check");
        int amount = main.world.planet[0].map.entitySystem.hiddenEntities.Count;
        for(int i = 0; i < amount; i++)
        {
            if (Vector2.Distance(player.transform.position, main.world.planet[0].map.entitySystem.hiddenEntities[i].position) <= respawnDistance)
            {
                print("respawn");
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
                print("respawned");
            }
        }
    }

    public void DespawnCheck()
    {
        print("despawn check");
        int amount = main.world.planet[0].map.entitySystem.visibleEntities.Count;
        for(int i = 0; i < amount; i++)
        {
            for(int t = 0; t < amount; t++)
            {
                print($"{main.world.planet[0].map.entitySystem.visibleEntities[t]}");
            }
            print(i);
            var n = main.world.planet[0].map.entitySystem.visibleEntities[i].position;
            print($"{i} success");
            if (Vector2.Distance(player.transform.position, main.world.planet[0].map.entitySystem.visibleEntities[i].position) >= despawnDistance)
            {
                print("despawn");
                var entityData = main.world.planet[0].map.entitySystem.visibleEntities[i];
                main.world.planet[0].map.entitySystem.visibleEntities.RemoveAt(i);
                main.world.planet[0].map.entitySystem.hiddenEntities.Add(entityData);
                Destroy(main.world.planet[0].map.entitySystem.spawnedEntityGameObject[i]);
                main.world.planet[0].map.entitySystem.spawnedEntityGameObject.RemoveAt(i);
                print("despawned");
            }
        }
    }
}
