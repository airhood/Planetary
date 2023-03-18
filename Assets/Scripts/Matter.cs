using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatterType
{
    Solid, Liquid, Gas
}

public enum Matters
{
    vacuum, stone
}

[CreateAssetMenu(menuName = "Matter", fileName = "new Matter", order = 30)]
public class Matter : ScriptableObject
{
    public short id;
    public string name;
    public string description;

    public MatterType matterType;

    public short hardness;

    public RuleTile tile;

    public bool canBePlacedAsBlock;

    public bool canBeConvertedToItem;
    public short itemID;
}
