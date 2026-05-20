using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text countText;

    private ItemData currentItem;
    private int count;

    // สีเงาดำตอนยังไม่มีของ
    public Color lockedColor = new Color(0, 0, 0, 0.5f);
    public bool isLocked = true; // ล็อคไว้ก่อน

    void Start()
    {
        UpdateUI();
    }
    public ItemData GetItem()
    {
        return currentItem;
    }

    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;
        count = amount;
        isLocked = false;
        UpdateUI();
    }

    public void AddCount(int amount)
    {
        count += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (isLocked)
        {
            itemIcon.sprite = null;
            itemIcon.color = lockedColor; // เงาดำ
            countText.text = "";
        }
        else if (currentItem != null)
        {
            itemIcon.sprite = currentItem.icon;
            itemIcon.color = Color.white;
            countText.text = count.ToString();
        }
    }
}