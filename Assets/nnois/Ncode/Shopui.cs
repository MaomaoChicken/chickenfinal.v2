using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI หลักของร้านค้า — ใช้ ShopItemData แยกจาก ItemData เดิม
/// ปิด Inventory เดิมอัตโนมัติเมื่อเปิดร้าน
public class ShopUI : MonoBehaviour
{
    [Header("Shop Panel")]
    public GameObject shopPanel;
    public Image merchantPortraitImage;
    public TextMeshProUGUI merchantNameText;
    public TextMeshProUGUI goldText;

    [Header("Tabs")]
    public Button tabBuyButton;
    public Button tabSellButton;
    public Button closeButton;

    [Header("Item List")]
    public Transform itemContainer;
    public GameObject itemSlotPrefab;

    [Header("สินค้าพ่อค้า (ใช้ ShopItemData)")]
    public List<ShopItemData> merchantItems = new List<ShopItemData>();

    [Header("Negotiation Panel")]
    public Negotiationui negotiationUI;

    // เชื่อม Inventory เดิม
    private Inventory inventorySystem;
    private bool isSellMode = false;

    // ====================================================
    void Start()
    {
        shopPanel.SetActive(false);
        tabBuyButton.onClick.AddListener(() => SetMode(false));
        tabSellButton.onClick.AddListener(() => SetMode(true));
        closeButton.onClick.AddListener(CloseShop);

        inventorySystem = FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        if (shopPanel.activeSelf && PlayerInventory.Instance != null)
            goldText.text = $"Gold: {PlayerInventory.Instance.gold}";

        if (shopPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
    }

    // ====================================================
    public void OpenShop(MerchantNPC merchant)
    {
        // ปิด Inventory/Crafting เดิมก่อน
        if (inventorySystem != null)
        {
            inventorySystem.mainInventory.SetActive(false);
            inventorySystem.mainCrafting.SetActive(false);
        }

        merchantNameText.text = merchant.merchantName;
        if (merchant.merchantPortrait != null)
            merchantPortraitImage.sprite = merchant.merchantPortrait;

        isSellMode = false;
        shopPanel.SetActive(true);
        RefreshItemList();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        negotiationUI?.Hide();
    }

    void SetMode(bool sell)
    {
        isSellMode = sell;
        RefreshItemList();
    }

    // ====================================================
    void RefreshItemList()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        if (isSellMode)
        {
            // ของที่ player มี — แปลง ItemData → ShopItemData ชั่วคราวเพื่อต่อราคา
            foreach (var kv in PlayerInventory.Instance.GetAllItems())
            {
                // หา ShopItemData ที่ตรงกับ ItemData นี้
                ShopItemData match = merchantItems.Find(s => s.itemData == kv.Key);
                if (match != null)
                    CreateSlot(match, kv.Value);
                else
                {
                    // item ที่พ่อค้าไม่รู้จัก → สร้าง ShopItemData ชั่วคราวด้วยราคา 0
                    ShopItemData temp = ScriptableObject.CreateInstance<ShopItemData>();
                    temp.itemData = kv.Key;
                    temp.basePrice = 50; // ราคา default เมื่อพ่อค้าไม่รู้จัก
                    CreateSlot(temp, kv.Value);
                }
            }
        }
        else
        {
            foreach (var shopItem in merchantItems)
                CreateSlot(shopItem, -1);
        }
    }

    void CreateSlot(ShopItemData shopItem, int qty)
    {
        GameObject go = Instantiate(itemSlotPrefab, itemContainer);
        ItemSlotUI slot = go.GetComponent<ItemSlotUI>();
        slot.Setup(shopItem, qty, isSellMode, OnDirectAction, OnTalkPrice);
    }

    // ====================================================
    void OnDirectAction(ShopItemData shopItem)
    {
        var inv = PlayerInventory.Instance;

        if (isSellMode)
        {
            if (!inv.HasItem(shopItem.itemData)) { Debug.Log("ไม่มี item!"); return; }
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(shopItem.basePrice);
            Debug.Log($"ขาย {shopItem.ItemName} +{shopItem.basePrice} G");
        }
        else
        {
            if (!inv.HasGold(shopItem.basePrice)) { Debug.Log("Gold ไม่พอ!"); return; }
            inv.SpendGold(shopItem.basePrice);
            inv.AddItem(shopItem.itemData);
            Debug.Log($"ซื้อ {shopItem.ItemName} -{shopItem.basePrice} G");
        }

        RefreshItemList();
    }

    void OnTalkPrice(ShopItemData shopItem)
    {
        var mode = isSellMode ? PriceNegotiator.ShopMode.Sell : PriceNegotiator.ShopMode.Buy;
        negotiationUI.StartNegotiation(shopItem, mode, OnNegotiationComplete);
    }

    void OnNegotiationComplete(bool success, ShopItemData shopItem, int finalPrice, PriceNegotiator.ShopMode mode)
    {
        if (!success) return;

        var inv = PlayerInventory.Instance;

        if (mode == PriceNegotiator.ShopMode.Buy)
        {
            if (!inv.HasGold(finalPrice)) { Debug.Log("Gold ไม่พอ!"); return; }
            inv.SpendGold(finalPrice);
            inv.AddItem(shopItem.itemData);
            Debug.Log($"ซื้อ {shopItem.ItemName} ต่อได้ {finalPrice} G");
        }
        else
        {
            if (!inv.HasItem(shopItem.itemData)) { Debug.Log("ไม่มี item!"); return; }
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(finalPrice);
            Debug.Log($"ขาย {shopItem.ItemName} ต่อได้ {finalPrice} G");
        }

        RefreshItemList();
    }
}