using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MatterType
{
    Solid, Liquid, Gas
}

public enum Matters
{
    vacuum, stone, Dirt, Coal, Copper, Iron, Gold, Diamond
}

[CreateAssetMenu(menuName = "World/Matter", fileName = "new Matter", order = 30)]
public class Matter : ScriptableObject
{
    [Header("Info")]
    public short id;
    public new string name;
    public string description;
    
    public MatterType matterType;
    
    [Header("Solid")]
    public short hardness;
    public TileBase tile;

    public bool canBePlacedAsBlock;
    public bool canBeConvertedToItem;
    public short itemID;

    [Header("Ore")]
    public float rarity;
    public float size;
}
