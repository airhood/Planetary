using System;
using UnityEngine;

public class ItemFunctionManager : MonoBehaviour
{
    public static Player player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public static void InvokeItemFunc(short itemID, string functionTag, object[] parameters)
    {
        string itemName = Main.data.itemList[itemID].name;
        if (Main.data.itemList[itemID].itemTag == ItemTag.Food) ItemFunction.EatFood(player, itemID);
        string methodName = itemName + "__" + functionTag;
        if (!typeof(ItemFunction).GetMethod(methodName).IsStatic)
        {
            Log.LogError("ItemFunctionManager.InvokeItemFunc: ItemFunction isn't static");
            return;
        }
        object[] _parameters = new object[parameters.Length + 1];
        _parameters[0] = player;
        try
        {
            for(int i = 1; i < _parameters.Length; i++)
            {
                _parameters[i] = parameters[i - 1];
            }
        } catch (Exception ex)
        {
            Log.LogError($"ItemFuncionManager.InvokeItemFunc: Error grouping function parameters. Error code: {ex}");
            return;
        }
        typeof(ItemFunction).GetMethod(methodName).Invoke(null, _parameters);
    }
}
