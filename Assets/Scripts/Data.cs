using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Data", fileName = "new Data", order = 1)]
public class Data : ScriptableObject
{
    public List<Block> blockList = new List<Block>();
    public List<Matter> matterList = new List<Matter>();
    public List<Item> itemList = new List<Item>();
    public List<Recipe> recipeList = new List<Recipe>();
}
