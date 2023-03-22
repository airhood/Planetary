using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Item, Matter, Block
}

[CreateAssetMenu(menuName = "Item", fileName = "new Item", order = 32)]
public class Item : ScriptableObject
{
    public short id;
    public string name;
    public string description;
    public Sprite image;
    public ItemType type;
    public short placeableID;
}
