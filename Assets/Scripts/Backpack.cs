using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class Slot
{
    public short itemID;
    public byte amount;

    public Slot(short itemID, byte amount)
    {
        this.itemID = itemID;
        this.amount = amount;
    }
}

public class Drill
{
    
}

[System.Serializable]
public class Backpack : MonoBehaviour
{
    public int BackPackLevel;

    //public List<Slot> slots = Enumerable.Repeat(new Slot(null, 0), 10).ToList();
    public List<Slot> slots = new List<Slot>();

    public byte index { get; private set; }

    public GameObject hotBar;

    public GameObject hotBarFrame;

    public GameObject inventory;

    public Player player;

    void Start()
    {
        UpdateHotBarUI();
        UpdateInventoryUI();
    }

    public void ChangeHotBarPos(byte index)
    {
        this.index = index;
        UpdateHotBarUI();
    }

    public void UpdateHotBarUI()
    {
        for(int i = 0; i < 10; i++)
        {
            var hotBarSlot = hotBar.transform.GetChild(i);
            if (slots[i].amount != 0)
            {
                hotBarSlot.GetChild(0).GetComponent<Image>().sprite = Main.itemList[slots[i].itemID].image;
                hotBarSlot.GetChild(1).gameObject.SetActive(true);
                hotBarSlot.GetChild(1).GetComponent<Text>().text = slots[i].amount.ToString();
            }
            else
            {
                hotBarSlot.GetChild(0).GetComponent<Image>().sprite = null;
                hotBarSlot.GetChild(1).gameObject.SetActive(false);
            }
        }

        if (player.toolMode == ToolMode.None)
        {
            hotBarFrame.transform.position = hotBar.transform.GetChild(index).position;
            hotBarFrame.SetActive(true);
        }
        else
        {
            hotBarFrame.SetActive(false);
        }
    }

    public void UpdateInventoryUI()
    {
        for(int i = 10; i < 40; i++)
        {
            var inventorySlot = inventory.transform.GetChild(i - 10);
            print($"i:{i}, slots[i].amount:{slots[i].amount}");
            if (slots[i].amount != 0)
            {
                inventorySlot.GetChild(0).GetComponent<Image>().sprite = Main.itemList[slots[i].itemID].image;
                inventorySlot.GetChild(1).gameObject.SetActive(true);
                inventorySlot.GetChild(1).GetComponent<Text>().text = slots[i].amount.ToString();
            }
            else
            {
                inventorySlot.GetChild(0).GetComponent<Image>().sprite = null;
                inventorySlot.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void AddItemToBackpack(short itemID, ushort amount)
    {
        byte maxStackAmount = Main.itemList[itemID].maxStackAmount;
        ushort amountLeft = amount;
        for(int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemID == itemID)
            {
                if (slots[i].amount + amountLeft <= maxStackAmount)
                {
                    slots[i].amount += (byte)amountLeft;
                    UpdateHotBarUI();
                    UpdateInventoryUI();
                    return;
                }
                else
                {
                    byte prevAmount = slots[i].amount;
                    slots[i].amount = maxStackAmount;
                    amountLeft -= (byte)(maxStackAmount - prevAmount);
                }
            }
            else if (slots[i].amount == 0)
            {
                slots[i].itemID = itemID;
                if (amountLeft <= maxStackAmount)
                {
                    slots[i].amount = (byte)amountLeft;
                    UpdateHotBarUI();
                    UpdateInventoryUI();
                    return;
                }
                else
                {
                    slots[i].amount = maxStackAmount;
                    amountLeft -= maxStackAmount;
                }
            }
        }
        UpdateHotBarUI();
        UpdateInventoryUI();
    }

    public void RemoveItem(byte index, byte amount)
    {
        if (slots[index].amount - amount < 0)
        {
            slots[index].amount = 0;
            UpdateHotBarUI();
            UpdateInventoryUI();
            return;
        }
        slots[index].amount -= amount;
        UpdateHotBarUI();
        UpdateInventoryUI();
    }

    public void AutoBackpackSerialization()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            for(int n = 0; n < i; n++)
            {
                if (slots[n].amount == 0)
                {
                    slots[n].itemID = slots[i].itemID;
                    slots[n].amount = slots[i].amount;
                    slots[i].itemID = -1;
                    slots[i].amount = 0;
                    break;
                }
            }
        }
    }
}
