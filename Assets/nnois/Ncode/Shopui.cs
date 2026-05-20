using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Panel")]
    public GameObject shopPanel;
    public Image merchantPortraitImage;
    public TextMeshProUGUI merchantNameText;

    [Header("ปุ่มหน้าหลัก")]
    public Button buyPageButton;
    public Button sellPageButton;
    public Button closeButton;

    [Header("BuyPanel")]
    public GameObject buyPanel;
    public Transform buyItemContainer;
    public Button buyBackButton;

    [Header("SellPanel")]
    public GameObject sellPanel;
    public Transform sellItemContainer;
    public Button sellBackButton;

    [Header("ปุ่ม Action (อยู่ด้านล่าง BuyPanel/SellPanel)")]
    public Button actionSellButton;   // ปุ่ม Sell/Buy
    public Button actionTalkButton;   // ปุ่ม Talk
    public TextMeshProUGUI actionSellLabel;

    [Header("Item Prefab")]
    public GameObject itemSlotPrefab;

    [Header("สินค้าพ่อค้า")]
    public List<ShopItemData> merchantItems = new List<ShopItemData>();

    [Header("Negotiation")]
    public Negotiationui negotiationui;

    private Inventory inventorySystem;
    private ItemSlotUI selectedSlot;
    private bool isSellMode;

    void Start()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);

        if (buyPageButton) buyPageButton.onClick.AddListener(OpenBuyPanel);
        if (sellPageButton) sellPageButton.onClick.AddListener(OpenSellPanel);
        if (closeButton) closeButton.onClick.AddListener(CloseShop);
        if (buyBackButton) buyBackButton.onClick.AddListener(BackToMain);
        if (sellBackButton) sellBackButton.onClick.AddListener(BackToMain);

        // ✅ ปุ่ม Action ด้านล่าง
        if (actionSellButton) actionSellButton.onClick.AddListener(OnActionSell);
        if (actionTalkButton) actionTalkButton.onClick.AddListener(OnActionTalk);

        SetActionButtons(false);
        inventorySystem = FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        bool anyOpen = (shopPanel != null && shopPanel.activeSelf)
                    || (buyPanel != null && buyPanel.activeSelf)
                    || (sellPanel != null && sellPanel.activeSelf);
        if (!anyOpen) return;
        if (Input.GetKeyDown(KeyCode.Escape)) CloseShop();
    }

    public void OpenShop(MerchantNPC merchant)
    {
        if (merchant == null) return;
        if (inventorySystem != null)
        {
            if (inventorySystem.mainInventory != null) inventorySystem.mainInventory.SetActive(false);
        }
        if (merchantNameText != null) merchantNameText.text = merchant.merchantName;
        if (merchantPortraitImage != null && merchant.merchantPortrait != null)
            merchantPortraitImage.sprite = merchant.merchantPortrait;

        if (shopPanel) shopPanel.SetActive(true);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        selectedSlot = null;
        SetActionButtons(false);
    }

    void OpenBuyPanel()
    {
        isSellMode = false;
        if (shopPanel) shopPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(true);
        if (actionSellLabel) actionSellLabel.text = "Buy";
        selectedSlot = null;
        SetActionButtons(false);
        RefreshList(buyItemContainer, false);
    }

    void OpenSellPanel()
    {
        isSellMode = true;
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(true);
        if (actionSellLabel) actionSellLabel.text = "Sell";
        selectedSlot = null;
        SetActionButtons(false);
        RefreshList(sellItemContainer, true);
    }

    void BackToMain()
    {
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        if (shopPanel) shopPanel.SetActive(true);
        negotiationui?.Hide();
        selectedSlot = null;
        SetActionButtons(false);
    }

    public void CloseShop()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (buyPanel) buyPanel.SetActive(false);
        if (sellPanel) sellPanel.SetActive(false);
        negotiationui?.Hide();
        selectedSlot = null;
        SetActionButtons(false);
    }

    void RefreshList(Transform container, bool isSell)
    {
        if (container == null || itemSlotPrefab == null) return;
        foreach (Transform child in container) Destroy(child.gameObject);
        if (PlayerInventory.Instance == null) return;

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
                CreateSlot(container, match, kv.Value);
            }
        }
        else
        {
            foreach (var shopItem in merchantItems)
            {
                if (shopItem == null) continue;
                CreateSlot(container, shopItem, -1);
            }
        }
    }

    void CreateSlot(Transform container, ShopItemData shopItem, int qty)
    {
        if (shopItem == null) return;
        GameObject go = Instantiate(itemSlotPrefab, container);
        ItemSlotUI slot = go.GetComponent<ItemSlotUI>();
        if (slot == null) return;
        // ✅ Setup ใหม่ — ส่ง callback onSelected แทน
        slot.Setup(shopItem, qty, OnSlotSelected);
    }

    // ✅ เมื่อกด slot → highlight + เปิดปุ่ม Action
    void OnSlotSelected(ItemSlotUI slot)
    {
        // deselect เดิม
        if (selectedSlot != null) selectedSlot.SetSelected(false);
        selectedSlot = slot;
        slot.SetSelected(true);
        SetActionButtons(true);
        Debug.Log($"Selected: {slot.Data?.ItemName}");
    }

    void SetActionButtons(bool show)
    {
        if (actionSellButton) actionSellButton.interactable = show;
        if (actionTalkButton) actionTalkButton.interactable = show;
    }

    // ✅ กดปุ่ม Sell/Buy ด้านล่าง
    void OnActionSell()
    {
        if (selectedSlot == null || selectedSlot.Data == null) return;
        var inv = PlayerInventory.Instance;
        var shopItem = selectedSlot.Data;

        if (isSellMode)
        {
            if (!inv.HasItem(shopItem.itemData)) { Debug.Log("ไม่มี item!"); return; }
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(shopItem.basePrice);
            Debug.Log($"ขาย {shopItem.ItemName} +{shopItem.basePrice} G");
            selectedSlot = null;
            SetActionButtons(false);
            RefreshList(sellItemContainer, true);
        }
        else
        {
            if (!inv.HasGold(shopItem.basePrice)) { Debug.Log("Gold ไม่พอ!"); return; }
            inv.SpendGold(shopItem.basePrice);
            inv.AddItem(shopItem.itemData);
            Debug.Log($"ซื้อ {shopItem.ItemName} -{shopItem.basePrice} G");
            selectedSlot = null;
            SetActionButtons(false);
            RefreshList(buyItemContainer, false);
        }
    }

    // ✅ กดปุ่ม Talk ด้านล่าง
    void OnActionTalk()
    {
        if (selectedSlot == null || selectedSlot.Data == null || negotiationui == null) return;
        var mode = isSellMode ? PriceNegotiator.ShopMode.Sell : PriceNegotiator.ShopMode.Buy;
        Debug.Log($"Talk: {selectedSlot.Data.ItemName} mode={mode}");
        negotiationui.StartNegotiation(selectedSlot.Data, mode, OnNegotiationComplete);
    }

    void OnNegotiationComplete(bool success, ShopItemData shopItem, int finalPrice, PriceNegotiator.ShopMode mode)
    {
        if (!success || shopItem == null || PlayerInventory.Instance == null) return;
        var inv = PlayerInventory.Instance;

        if (mode == PriceNegotiator.ShopMode.Buy)
        {
            if (!inv.HasGold(finalPrice)) return;
            inv.SpendGold(finalPrice);
            inv.AddItem(shopItem.itemData);
            RefreshList(buyItemContainer, false);
        }
        else
        {
            if (!inv.HasItem(shopItem.itemData)) return;
            inv.RemoveItem(shopItem.itemData);
            inv.EarnGold(finalPrice);
            RefreshList(sellItemContainer, true);
        }
        selectedSlot = null;
        SetActionButtons(false);
    }
}