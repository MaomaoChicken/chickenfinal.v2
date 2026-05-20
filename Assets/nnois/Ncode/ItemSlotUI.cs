using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// กดที่ slot → เปลี่ยนสี (selected)
/// ShopUI จะอ่าน selectedSlot แล้วให้ปุ่ม Sell/Talk ด้านล่างทำงาน
/// </summary>
public class ItemSlotUI : MonoBehaviour
{
    [Header("Display")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    [Header("สี")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color selectedColor = new Color(1f, 0.8f, 0.2f, 0.8f);

    public ShopItemData Data { get; private set; }
    public bool IsSelected { get; private set; }

    private Image bg;
    private Action<ItemSlotUI> onSelected;

    void Awake()
    {
        bg = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        bg.color = normalColor;

        Button btn = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Select);
    }

    public void Setup(ShopItemData shopItem, int qty, Action<ItemSlotUI> selectCallback)
    {
        if (shopItem == null) return;
        Data = shopItem;
        onSelected = selectCallback;
        IsSelected = false;
        bg.color = normalColor;

        if (itemIcon != null)
        {
            itemIcon.enabled = shopItem.Icon != null;
            if (shopItem.Icon != null) itemIcon.sprite = shopItem.Icon;
        }
        if (itemNameText != null) itemNameText.text = shopItem.ItemName;
        if (itemPriceText != null) itemPriceText.text = $"{shopItem.basePrice} G";
    }

    public void Select()
    {
        onSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        bg.color = selected ? selectedColor : normalColor;
    }
}