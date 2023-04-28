using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    DroppedItem
}

[System.Serializable]
public class EntityData
{
    public Vector2 position;
    public EntityType type;
    public int relatedID;

    public EntityData(Vector2 position, EntityType type, int relativeID)
    {
        this.position = position;
        this.type = type;
        this.relatedID = relativeID;
    }
}
