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

    void Start()
    {
        for(int i = 0; i < 40; i++)
        {
            slots.Add(new Slot(-1, 0));
        }
        updateHotBarUI();
    }

    public void changeHotBarPos(byte index)
    {
        this.index = index;
        updateHotBarUI();
    }

    public void updateHotBarUI()
    {
        for(int i = 0; i < 10; i++)
        {
            if (slots[i].amount != 0)
            {
                hotBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Main.itemList[(int)slots[i].itemID].image;
                hotBar.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                hotBar.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = slots[i].amount.ToString();
            }
            else
            {
                hotBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
                hotBar.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }

        hotBarFrame.transform.position = hotBar.transform.GetChild(index).position;
    }

    public void addItemToBackpack(short itemID, ushort amount)
    {
        ushort amountLeft = amount;
        for(int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemID == itemID)
            {
                if (slots[i].amount + amountLeft <= 255)
                {
                    slots[i].amount += (byte)amountLeft;
                    updateHotBarUI();
                    return;
                }
                else
                {
                    byte prevAmount = slots[i].amount;
                    slots[i].amount = 255;
                    amountLeft -= (byte)(255 - prevAmount);
                }
            }
            else if (slots[i].amount == 0)
            {
                slots[i].itemID = itemID;
                if (amountLeft <= 255)
                {
                    slots[i].amount = (byte)amountLeft;
                    updateHotBarUI();
                    return;
                }
                else
                {
                    slots[i].amount = 255;
                    amountLeft -= 255;
                }
            }
        }
        updateHotBarUI();
    }

    public void autoBackpackSerialization()
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
