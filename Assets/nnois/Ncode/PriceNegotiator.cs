using UnityEngine;

public class PriceNegotiator
{
    private int currentPrice;
    private ShopItemData shopItem;
    private ShopMode mode;

    private int minAcceptable;
    private int maxAcceptable;

    private int roundCount = 0;
    private const int MAX_ROUNDS = 3;

    public bool IsFinished { get; private set; } = false;
    public bool PlayerWon { get; private set; } = false;
    public int FinalPrice { get; private set; }

    public enum ShopMode { Buy, Sell }

    public PriceNegotiator(ShopItemData shopItem, ShopMode mode)
    {
        this.shopItem = shopItem;
        this.mode = mode;
        currentPrice = shopItem.basePrice;
        minAcceptable = Mathf.RoundToInt(shopItem.basePrice * shopItem.minPriceMultiplier);
        maxAcceptable = Mathf.RoundToInt(shopItem.basePrice * shopItem.maxPriceMultiplier);
    }

    public int GetCurrentPrice() => currentPrice;
    public int GetRound() => roundCount;
    public int GetMaxRounds() => MAX_ROUNDS;

    public string ProposePrice(int proposed)
    {
        roundCount++;
        return mode == ShopMode.Buy
            ? HandleBuy(proposed)
            : HandleSell(proposed);
    }

    // Player ซื้อ → อยากได้ถูก → เสนอต่ำ
    string HandleBuy(int proposed)
    {
        float stub = shopItem.merchantStubborn / 100f;

        if (proposed >= currentPrice)
        {
            Finish(true, proposed);
            return $"โอเค! ขายที่ {proposed} G ได้เลย!";
        }
        if (proposed < minAcceptable)
        {
            if (roundCount >= MAX_ROUNDS) { Finish(false, currentPrice); return $"ขอโทษนะ รับไม่ได้จริงๆ ราคาที่ขายได้คือ {currentPrice} G"; }
            return "ต่ำไปมากเลย!";
        }

        if (Random.value < (1f - stub * 0.8f) || roundCount >= MAX_ROUNDS)
        {
            int cut = Mathf.RoundToInt((currentPrice - proposed) * Random.Range(0.3f, 0.7f));
            currentPrice = Mathf.Max(proposed, currentPrice - cut);
            if (currentPrice <= proposed || roundCount >= MAX_ROUNDS) { Finish(true, currentPrice); return $"เอาวะ {currentPrice} G แต่อย่าบอกใครนะ!"; }
            return $"ลดได้แค่ {currentPrice} G ยังไงล่ะ?";
        }
        return Refusal();
    }

    // Player ขาย → อยากได้แพง → เสนอสูง
    string HandleSell(int proposed)
    {
        float stub = shopItem.merchantStubborn / 100f;

        if (proposed <= currentPrice)
        {
            Finish(true, proposed);
            return $"ดีเลย รับซื้อที่ {proposed} G!";
        }
        if (proposed > maxAcceptable)
        {
            if (roundCount >= MAX_ROUNDS) { Finish(false, currentPrice); return $"รับไม่ไหว ขายที่ {currentPrice} G ได้นะ"; }
            return "แพงไปมากเลย!";
        }

        if (Random.value < (1f - stub * 0.8f) || roundCount >= MAX_ROUNDS)
        {
            int raise = Mathf.RoundToInt((proposed - currentPrice) * Random.Range(0.3f, 0.7f));
            currentPrice = Mathf.Min(proposed, currentPrice + raise);
            if (currentPrice >= proposed || roundCount >= MAX_ROUNDS) { Finish(true, currentPrice); return $"โอเค {currentPrice} G แล้วกันครับ"; }
            return $"ให้ได้ {currentPrice} G น่ะครับ";
        }
        return Refusal();
    }

    string Refusal()
    {
        string[] lines = { "ไม่ได้!", "ฉันให้แกคิดใหม่อีกที", "ราคาตลาดเป็นแบบนี้เป็นแบบนี้อยู่แล้วนะครับ", "ล้อเล่นใช่ไหม?" };
        return lines[Random.Range(0, lines.Length)];
    }

    void Finish(bool won, int price) { IsFinished = true; PlayerWon = won; FinalPrice = price; }

    public void AcceptCurrentPrice() => Finish(true, currentPrice);
    public void Cancel() => Finish(false, currentPrice);
}