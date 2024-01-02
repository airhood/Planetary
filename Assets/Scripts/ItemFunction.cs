using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunction : MonoBehaviour
{
    #region SpecialFunctions
    public static void EatFood(Player player, short itemID)
    {
        player.playerInfo.calories += Main.data.itemList[itemID].calorieAddAmount;
    }

    public static void Attack(Player player, short itemID)
    {
        player.Attack(Main.data.itemList[itemID].attackDamage, Main.data.itemList[itemID].attackRange);
    }
    #endregion
}
