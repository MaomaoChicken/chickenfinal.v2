using System.Collections.Generic;
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Starting Gold")]
    public int gold = 500;

    // ใช้ ItemData เดิมเป็น key
    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool HasGold(int amount) => gold >= amount;
    public void SpendGold(int amount) => gold = Mathf.Max(0, gold - amount);
    public void EarnGold(int amount) => gold += amount;

    public void AddItem(ItemData item, int qty = 1)
    {
        if (!items.ContainsKey(item)) items[item] = 0;
        items[item] += qty;
    }

    public bool HasItem(ItemData item, int qty = 1) =>
        items.ContainsKey(item) && items[item] >= qty;

    public bool RemoveItem(ItemData item, int qty = 1)
    {
        if (!HasItem(item, qty)) return false;
        items[item] -= qty;
        if (items[item] <= 0) items.Remove(item);
        return true;
    }

    public int GetItemCount(ItemData item) =>
        items.ContainsKey(item) ? items[item] : 0;

    public Dictionary<ItemData, int> GetAllItems() => items;
}