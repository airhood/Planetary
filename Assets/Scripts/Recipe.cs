using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Crafter
{
    Backpack, CraftingTable, MetalWorkbench, EngineeringWorkbench, PipeWorkbench, ElectricWorkbench, PlasticWorkbench, RocketWorkbench
}

[CreateAssetMenu(menuName = "World/Recipe", fileName = "new Recipe")]
public class Recipe : ScriptableObject
{
    public short id;
    public List<ItemStack> requiredItems;
    public ItemStack resultItem;
    public Crafter availableCrafter;
}
