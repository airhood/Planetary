using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType:byte
{
    Item, Matter, Block
}

public enum ItemTag
{
    None, Food
}

[CreateAssetMenu(menuName = "World/Item", fileName = "new Item", order = 32)]
public class Item : ScriptableObject
{
    [Header("Info")]
    public short id;
    public string display_name;
    public new string name;
    public string description;
    public Sprite image;
    public ItemType type;
    public byte maxStackAmount;

    [Header("Item")]
    public bool isUsable;
    public bool isRemoveOnUse;
    public ItemTag itemTag;

    public short recipeID;

    [Header("Item<Food>")]
    public byte calorieAddAmount;

    [Header("Item<Weapon>")]
    public float attackDamage;
    public float attackRange;

    [Header("Block/Matter")]
    public short placeableID;
}
