using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    public Canvas canvas;

    public bool isDragging = false;

    byte lastSlot;

    Vector2 mouse_position;
    Vector2 previous_mouse_position;
    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;

    List<GameObject> clicked_elements;

    GameObject drag_element;

    ItemDragUI drag_element_itemDragUI;

    bool dragged = false;

    public GameObject itemDragUI;

    void Start()
    {
        ui_raycaster = canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
        clicked_elements = new List<GameObject>();
        UpdateHotBarUI();
        UpdateInventoryUI();
    }

    void Update()
    {
        MouseDragUI();
    }

    private void MouseDragUI()
    {
        mouse_position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            DetectUI();
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            DragElement();
            dragged = true;
        }
        else if (dragged)
        {
            isDragging = false;
            dragged = false;
            ApplyItemMove();
        }

        previous_mouse_position = mouse_position;
    }

    private void DetectUI()
    {
        GetUIElementsClicked();

        if (clicked_elements.Count > 0)
        {
            for(int i= 0; i < clicked_elements.Count; i++)
            {
                if (clicked_elements[i].CompareTag("ItemIconUI"))
                {
                    byte slot;
                    if (byte.TryParse(clicked_elements[i].name, out slot))
                    {
                        slot--;
                        if (slots[slot].amount <= 0) return;
                        lastSlot = slot;
                        isDragging = true;
                        drag_element = Instantiate(itemDragUI);
                        drag_element.transform.parent = canvas.transform;
                        drag_element.GetComponent<RectTransform>().position = Input.mousePosition;
                        drag_element_itemDragUI = drag_element.GetComponent<ItemDragUI>();
                        drag_element_itemDragUI.itemID = slots[slot].itemID;
                        drag_element_itemDragUI.amount = slots[slot].amount;
                        drag_element_itemDragUI.updateUI();
                        drag_element.transform.GetChild(0).GetComponent<Image>().sprite = Main.itemList[slots[slot].itemID].image;
                        slots[slot].itemID = 0;
                        slots[slot].amount = 0;
                        UpdateHotBarUI();
                        UpdateInventoryUI();
                    }
                    return;
                }
            }
        }
    }

    private void GetUIElementsClicked()
    {
        click_data.position = mouse_position;
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);

        clicked_elements.Clear();

        // Optimised version
        //clicked_elements = (from result in click_results select result.gameObject).ToList();

        // For version
        for(int i = 0; i < click_results.Count; i++)
        {
            clicked_elements.Add(click_results[i].gameObject);
        }
    }

    private void DragElement()
    {
        RectTransform element_rect = drag_element.GetComponent<RectTransform>();

        Vector2 drag_movement = mouse_position - previous_mouse_position;

        element_rect.anchoredPosition = element_rect.anchoredPosition + drag_movement;
    }

    private void ApplyItemMove()
    {
        click_data.position = mouse_position;
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);

        clicked_elements.Clear();

        for (int i = 0; i < click_results.Count; i++)
        {
            clicked_elements.Add(click_results[i].gameObject);
        }

        GameObject clickedElement;

        if (clicked_elements.Count > 0)
        {
            for (int i = 0; i < clicked_elements.Count; i++)
            {
                if (clicked_elements[i].CompareTag("ItemIconUI"))
                {
                    clickedElement = clicked_elements[i];
                    byte slot;
                    print($"clickedElement.name: {clickedElement.name}");
                    if (byte.TryParse(clickedElement.name, out slot))
                    {
                        print("alalalal");
                        short slotItemID = slots[slot - 1].itemID;
                        byte slotAmount = slots[slot - 1].amount;
                        if (drag_element_itemDragUI.itemID == slotItemID)
                        {
                            if (slotAmount + drag_element_itemDragUI.amount > 255)
                            {
                                // ��Ŭ��
                                byte addAmount = (byte)(255 - slots[slot - 1].amount);
                                slots[slot - 1].amount = 255;
                                slots[lastSlot].itemID = drag_element_itemDragUI.itemID;
                                slots[lastSlot].amount = (byte)(slotAmount - addAmount);
                            }
                            else
                            {
                                slots[slot - 1].amount += drag_element_itemDragUI.amount;
                                Destroy(drag_element);
                            }
                        }
                        else
                        {
                            print("fnfnfnfn");
                            slots[slot - 1].itemID = drag_element_itemDragUI.itemID;
                            slots[slot - 1].amount = drag_element_itemDragUI.amount;
                            drag_element_itemDragUI.itemID = slotItemID;
                            drag_element_itemDragUI.amount = slotAmount;
                            if (drag_element_itemDragUI.amount == 0)
                            {
                                Destroy(drag_element);
                            }
                            else
                            {
                                drag_element_itemDragUI.updateUI();
                            }
                        }

                        UpdateHotBarUI();
                        UpdateInventoryUI();
                    }
                    return;
                }
            }
        }

        slots[lastSlot].itemID = drag_element_itemDragUI.itemID;
        slots[lastSlot].amount = drag_element_itemDragUI.amount;
        Destroy(drag_element);
        drag_element_itemDragUI = null;
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
