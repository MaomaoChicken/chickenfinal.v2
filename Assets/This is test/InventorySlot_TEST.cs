using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventorySlot_TEST : MonoBehaviour
{
    public Image itemIcon;

    public ItemData currentItem;

    public void SetItem(ItemData item, int qty)
    {
        currentItem = item;

        if (itemIcon != null)
        {
            itemIcon.enabled = item != null && item.icon != null;
            if (item != null && item.icon != null)
                itemIcon.sprite = item.icon;
        }

    }

    public void Clear()
    {
        currentItem = null;
        if (itemIcon != null) itemIcon.enabled = false;
    }
}