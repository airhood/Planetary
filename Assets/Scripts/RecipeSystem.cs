using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSystem : MonoBehaviour
{
    public Backpack backpack;

    public short GetRecipeID(short itemID)
    {
        return Main.itemList[itemID].recipeID;
    }

    public List<ItemStack> GetRecipeRequirements(short recipeID, short amount)
    {
        var requiredItems = Main.recipeList[recipeID].requiredItems;
        for(int i = 0; i < requiredItems.Count; i++)
        {
            requiredItems[i].amount *= amount;
            requiredItems[i].amount *= -1;
        }
        return Main.recipeList[recipeID].requiredItems;
    }

    public void CraftItem(short recipeID, short amount)
    {
        backpack.AddItemToBackpack(new ItemStack(Main.recipeList[recipeID].resultItem.itemID, 
            Main.recipeList[recipeID].resultItem.amount * amount));
    }

    public bool CheckCraftAvailable(short recipeID, short amount)
    {
        var requiredItems = GetRecipeRequirements(recipeID, amount);
        for(int i = 0; i < requiredItems.Count; i++)
        {
            if (!backpack.CheckItemExist(requiredItems[i])) return false;
        }
        return true;
    }
}
