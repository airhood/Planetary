using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunction : MonoBehaviour
{
    public static void EatFood(short itemID)
    {
        ItemFunctionManager.player.calories += Main.itemList[itemID].calorieAddAmount;
    }
}
