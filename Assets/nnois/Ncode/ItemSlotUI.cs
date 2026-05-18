using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Prefab แต่ละ slot ในร้าน — รับ ShopItemData
public class ItemSlotUI : MonoBehaviour
{
    [Header("Display")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI itemQtyText;

    [Header("Buttons")]
    public Button sellButton;
    public Button talkButton;
    public TextMeshProUGUI sellButtonLabel;
    public TextMeshProUGUI talkButtonLabel;

    private Action<ShopItemData> onSell;
    private Action<ShopItemData> onTalk;
    private ShopItemData data;

    public void Setup(ShopItemData shopItem, int qty, bool isSellMode,
                      Action<ShopItemData> sellCallback,
                      Action<ShopItemData> talkCallback)
    {
        data = shopItem;
        onSell = sellCallback;
        onTalk = talkCallback;

        // แสดงข้อมูลจาก ItemData เดิมผ่าน ShopItemData
        if (itemIcon != null && shopItem.Icon != null)
            itemIcon.sprite = shopItem.Icon;

        itemNameText.text = shopItem.ItemName;
        itemPriceText.text = $"{shopItem.basePrice} G";

        if (itemQtyText != null)
            itemQtyText.text = qty < 0 ? "∞" : $"x{qty}";

        if (sellButtonLabel != null)
            sellButtonLabel.text = isSellMode ? "ขาย" : "ซื้อ";

        if (talkButtonLabel != null)
            talkButtonLabel.text = isSellMode ? "ต่อราคาขาย" : "ต่อราคาซื้อ";

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => onSell?.Invoke(data));

        talkButton.onClick.RemoveAllListeners();
        talkButton.onClick.AddListener(() => onTalk?.Invoke(data));
    }
}