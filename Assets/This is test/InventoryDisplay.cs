using System.Collections.Generic;
using UnityEngine;


public class InventoryDisplay : MonoBehaviour
{
    [Header("ถ้าไม่ลาก จะหา InventorySlot ใน children อัตโนมัติ")]
    public List<InventorySlot_TEST> slots = new List<InventorySlot_TEST>();

    void OnEnable()
    {
        // ทุกครั้งที่เปิด Inventory จะ refresh ใหม่
        Refresh();
    }

    public void Refresh()
    {
        // หา slot อัตโนมัติถ้าไม่ได้ลาก
        if (slots.Count == 0)
            slots.AddRange(GetComponentsInChildren<InventorySlot_TEST>());

        // เคลียร์ทุก slot ก่อน
        foreach (var slot in slots)
            slot.Clear();

        if (PlayerInventory.Instance == null) return;

        // ใส่ item ลงใน slot ทีละช่อง
        int i = 0;
        foreach (var kv in PlayerInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;
            slots[i].SetItem(kv.Key, kv.Value);
            i++;
        }
    }
}