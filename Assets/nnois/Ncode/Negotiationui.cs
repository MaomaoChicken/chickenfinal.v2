using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel ต่อราคา — รับ ShopItemData แทน ItemData
/// </summary>
public class Negotiationui : MonoBehaviour
{
    [Header("Panel")]
    public GameObject negotiationPanel;

    [Header("Merchant Side")]
    public Image merchantPortraitImage;
    public TextMeshProUGUI merchantDialogText;

    [Header("Item Info")]
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI currentPriceText;
    public TextMeshProUGUI basePriceText;

    [Header("Propose")]
    public Slider proposeSlider;
    public TextMeshProUGUI proposePriceText;
    public TextMeshProUGUI roundsText;

    [Header("Buttons")]
    public Button proposeButton;
    public Button acceptButton;
    public Button cancelButton;

    private PriceNegotiator negotiator;
    private ShopItemData currentShopItem;
    private PriceNegotiator.ShopMode currentMode;
    private Action<bool, ShopItemData, int, PriceNegotiator.ShopMode> onComplete;

    // ====================================================
    void Start()
    {
        if (negotiationPanel) negotiationPanel.SetActive(false);
        proposeButton.onClick.AddListener(OnPropose);
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
        if (proposeSlider != null)
            proposeSlider.onValueChanged.AddListener(v =>
            {
                if (proposePriceText) proposePriceText.text = $"เสนอ: {(int)v} G";
            });
    }

    // ====================================================
    public void StartNegotiation(
        ShopItemData shopItem,
        PriceNegotiator.ShopMode mode,
        Action<bool, ShopItemData, int, PriceNegotiator.ShopMode> onCompleteCallback)
    {
        currentShopItem = shopItem;
        currentMode = mode;
        onComplete = onCompleteCallback;
        negotiator = new PriceNegotiator(shopItem, mode);

        negotiationPanel.SetActive(true);

        if (itemIconImage != null && shopItem.Icon != null)
            itemIconImage.sprite = shopItem.Icon;

        itemNameText.text = shopItem.ItemName;
        basePriceText.text = $"ราคาตลาด: {shopItem.basePrice} G";

        // ตั้ง slider range
        if (proposeSlider != null)
        {
            proposeSlider.minValue = Mathf.RoundToInt(shopItem.basePrice * shopItem.minPriceMultiplier);
            proposeSlider.maxValue = Mathf.RoundToInt(shopItem.basePrice * shopItem.maxPriceMultiplier);
            proposeSlider.wholeNumbers = true;
            proposeSlider.value = mode == PriceNegotiator.ShopMode.Buy
                ? Mathf.RoundToInt(shopItem.basePrice * 0.8f)
                : Mathf.RoundToInt(shopItem.basePrice * 1.2f);
        }

        merchantDialogText.text = mode == PriceNegotiator.ShopMode.Buy
            ? $"ยินดีต้อนรับ! {shopItem.ItemName} ราคา {shopItem.basePrice} G ครับ~"
            : $"อยากขาย {shopItem.ItemName} เหรอ?";

        UpdateUI();
    }

    // ====================================================
    void OnPropose()
    {
        if (negotiator == null || negotiator.IsFinished) return;
        int proposed = proposeSlider != null ? (int)proposeSlider.value : negotiator.GetCurrentPrice();
        merchantDialogText.text = negotiator.ProposePrice(proposed);
        UpdateUI();
        if (negotiator.IsFinished) FinishNegotiation();
    }

    void OnAccept()
    {
        if (negotiator == null) return;
        negotiator.AcceptCurrentPrice();
        merchantDialogText.text = "ตกลง! ขอบคุณมากเลยครับ~";
        UpdateUI();
        FinishNegotiation();
    }

    void OnCancel()
    {
        negotiator?.Cancel();
        merchantDialogText.text = "โอเค มาคุยกันใหม่ได้เสมอนะครับ";
        Hide();
        onComplete?.Invoke(false, currentShopItem, 0, currentMode);
    }

    void FinishNegotiation()
    {
        proposeButton.interactable = false;
        acceptButton.interactable = false;
        onComplete?.Invoke(negotiator.PlayerWon, currentShopItem, negotiator.FinalPrice, currentMode);
        Invoke(nameof(Hide), 1.5f);
    }

    public void Hide()
    {
        if (negotiationPanel) negotiationPanel.SetActive(false);
        negotiator = null;
    }

    void UpdateUI()
    {
        if (negotiator == null) return;
        currentPriceText.text = $"ราคาปัจจุบัน: {negotiator.GetCurrentPrice()} G";
        roundsText.text = $"รอบที่ {negotiator.GetRound()} / {negotiator.GetMaxRounds()}";
        bool active = !negotiator.IsFinished;
        proposeButton.interactable = active;
        acceptButton.interactable = active;
        if (proposeSlider) proposeSlider.interactable = active;
    }
}