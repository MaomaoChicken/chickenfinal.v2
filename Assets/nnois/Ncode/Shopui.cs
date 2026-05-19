using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Panel (หน้าหลัก — ปุ่ม Buy/Sell/Close)")]
    public GameObject shopPanel;
    public Image merchantPortraitImage;
    public TextMeshProUGUI merchantNameText;
    public TextMeshProUGUI goldText;

    [Header("ปุ่มหน้าหลัก")]
    public Button buyPageButton;        // กดแล้วเปิด BuyPanel
    public Button sellPageButton;       // กดแล้วเปิด SellPanel
    public Button closeButton;          // ปิดร้าน

    [Header("BuyPanel (หน้าซื้อ — แยกออกมา)")]
    public GameObject buyPanel;
    public Transform buyItemContainer;  // Content ใน BuyPanel ScrollView
    public Button buyBackButton;        // ปุ่มกลับ

    [Header("SellPanel (หน้าขาย — แยกออกมา)")]
    public GameObject sellPanel;
    public Transform sellItemContainer; // Content ใน SellPanel ScrollView
    public Button sellBackButton;       // ปุ่มกลับ

    [Header("Item Prefab (ใช้ร่วมกันทั้งสองหน้า)")]
    public GameObject itemSlotPrefab;

    [Header("ขายให้พ่อค้า")]
    public List<ShopItemData> merchantItems = new List<ShopItemData>();

    [Header("Negotiation Panel")]
    public Negotiationui negotiationUI;

    private Inventory inventorySystem;

    void Start()
    {
        // ปิดทุก panel ก่อน
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);

        // หน้าหลัก
        if (buyPageButton) buyPageButton.onClick.AddListener(OpenBuyPanel);
        if (sellPageButton) sellPageButton.onClick.AddListener(OpenSellPanel);
        if (closeButton) closeButton.onClick.AddListener(CloseShop);

        // ปุ่มกลับในแต่ละหน้า
        if (buyBackButton) buyBackButton.onClick.AddListener(BackToMain);
        if (sellBackButton) sellBackButton.onClick.AddListener(BackToMain);

        inventorySystem = FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        bool anyOpen = (shopPanel != null && shopPanel.activeSelf)
                    || (buyPanel != null && buyPanel.activeSelf)
                    || (sellPanel != null && sellPanel.activeSelf);

        if (!anyOpen) return;

        if (goldText != null && PlayerInventory.Instance != null)
            goldText.text = $"Gold: {PlayerInventory.Instance.gold}";

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseShop();
    }

    // ====================================================
    // เปิดร้าน → แสดงหน้าหลักก่อน (มีแค่ปุ่ม Buy / Sell / Close)
    public void OpenShop(MerchantNPC merchant)
    {
        if (merchant == null) return;

        if (inventorySystem != null)
        {
            if (inventorySystem.mainInventory != null)
                inventorySystem.mainInventory.SetActive(false);
            if (inventorySystem.mainCrafting != null)
                inventorySystem.mainCrafting.SetActive(false);
        }

        if (merchantNameText != null)
            merchantNameText.text = merchant.merchantName;

        if (merchantPortraitImage != null && merchant.merchantPortrait != null)
            merchantPortraitImage.sprite = merchant.merchantPortrait;

        // เปิดเฉพาะหน้าหลัก
        if (shopPanel) shopPanel.SetActive(true);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
    }

    // ====================================================
    // กด "Buy" → ซ่อนหน้าหลัก เปิด BuyPanel
    void OpenBuyPanel()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(true);

        RefreshList(buyItemContainer, isSell: false);
    }

    // กด "Sell" → ซ่อนหน้าหลัก เปิด SellPanel
    void OpenSellPanel()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(true);

        RefreshList(sellItemContainer, isSell: true);
    }

    // ปุ่มกลับ → เปิดหน้าหลักอีกครั้ง
    void BackToMain()
    {
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        if (shopPanel) shopPanel.SetActive(true);
        negotiationUI?.Hide();
    }

    public void CloseShop()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        negotiationUI?.Hide();
    }

    // ====================================================
    void RefreshList(Transform container, bool isSell)
    {
        if (container == null || itemSlotPrefab == null) return;

        foreach (Transform child in container)
            Destroy(child.gameObject);

        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning("PlayerInventory.Instance is null!");
            return;
        }

        if (isSell)
        {
            foreach (var kv in PlayerInventory.Instance.GetAllItems())
            {
                if (kv.Key == null) continue;
                ShopItemData match = merchantItems.Find(s => s != null && s.itemData == kv.Key);
                if (match == null)
                {
                    ShopItemData temp = ScriptableObject.CreateInstance<ShopItemData>();
                    temp.itemData = kv.Key;
                    temp.basePrice = 50;
                    match = temp;
                }
                CreateSlot(container, match, kv.Value, isSell: true);
            }
        }
        else
        {
            foreach (var shopItem in merchantItems)
            {
                if (shopItem == null) continue;
                CreateSlot(container, shopItem, -1, isSell: false);
            }
        }
    }

    void CreateSlot(Transform container, ShopItemData shopItem, int qty, bool isSell)
    {
        if (shopItem == null) return;
        GameObject go = Instantiate(itemSlotPrefab, container);
        ItemSlotUI slot = go.GetComponent<ItemSlotUI>();
        if (slot == null) return;
        slot.Setup(shopItem, qty, isSell, OnDirectAction, OnTalkPrice);
    }

    // ====================================================
    void OnDirectAction(ShopItemData shopItem)
    {
        if (shopItem == null || PlayerInventory.Instance == null) return;
        var inv = PlayerInventory.Instance;

        // ตรวจว่าอยู่ใน SellPanel หรือ BuyPanel
        bool isSell = sellPanel != null && sellPanel.activeSelf;

        if (isSell)
        {
            if (!inv.HasItem(shopItem.itemData)) { Debug.Log("ไม่มี item!"); return; }
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(shopItem.basePrice);
            RefreshList(sellItemContainer, isSell: true);
        }
        else
        {
            if (!inv.HasGold(shopItem.basePrice)) { Debug.Log("Gold ไม่พอ!"); return; }
            inv.SpendGold(shopItem.basePrice);
            inv.AddItem(shopItem.itemData);
            RefreshList(buyItemContainer, isSell: false);
        }
    }

    void OnTalkPrice(ShopItemData shopItem)
    {
        if (shopItem == null || negotiationUI == null) return;
        bool isSell = sellPanel != null && sellPanel.activeSelf;
        var mode = isSell ? PriceNegotiator.ShopMode.Sell : PriceNegotiator.ShopMode.Buy;
        negotiationUI.StartNegotiation(shopItem, mode, OnNegotiationComplete);
    }

    void OnNegotiationComplete(bool success, ShopItemData shopItem, int finalPrice, PriceNegotiator.ShopMode mode)
    {
        if (!success || shopItem == null || PlayerInventory.Instance == null) return;
        var inv = PlayerInventory.Instance;

        if (mode == PriceNegotiator.ShopMode.Buy)
        {
            if (!inv.HasGold(finalPrice)) { Debug.Log("Gold ไม่พอ!"); return; }
            inv.SpendGold(finalPrice);
            inv.AddItem(shopItem.itemData);
            RefreshList(buyItemContainer, isSell: false);
        }
        else
        {
            if (!inv.HasItem(shopItem.itemData)) { Debug.Log("ไม่มี item!"); return; }
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(finalPrice);
            RefreshList(sellItemContainer, isSell: true);
        }
    }
}