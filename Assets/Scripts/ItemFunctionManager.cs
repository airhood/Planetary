using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunctionManager : MonoBehaviour
{
    public static Player player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public static void InvoketemFunc(short itemID, string itemName, string functionTag, object[] parameters)
    {
        if (Main.itemList[itemID].itemTag == ItemTag.Food) ItemFunction.EatFood(itemID);
        string methodName = itemName + "_" + functionTag;
        if (!typeof(ItemFunction).GetMethod(methodName).IsStatic)
        {
            Log.LogError("ItemFuncManager.InvokeItemFunc: ItemFunction isn't static");
            return;
        }

        typeof(ItemFunction).GetMethod(methodName).Invoke(null, parameters);
    }
}
