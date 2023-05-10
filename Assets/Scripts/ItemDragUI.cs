using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDragUI : MonoBehaviour
{
    public short itemID;
    public byte amount;

    public void updateUI()
    {
        if (itemID == 0) return;
        if (amount == 0) return;
        transform.GetChild(0).GetComponent<Image>().sprite = Main.itemList[itemID].image;
        transform.GetChild(1).GetComponent<Text>().text = amount.ToString();
    }
}
