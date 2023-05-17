using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public short itemID;
    public short amount;
}

[CreateAssetMenu(menuName = "World/Recipe", fileName = "new Recipe")]
public class Recipe : ScriptableObject
{
    public List<Items> requiredItems;
}
