using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] slots; // ลาก Slot ทั้งหมดมาใส่

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        // เช็คว่ามีของชิ้นนี้อยู่แล้วมั้ย
        foreach (var slot in slots)
        {
            if (!slot.isLocked && slot.GetItem() == item)
            {
                slot.AddCount(1);
                return;
            }
        }

        // หา slot ว่างแรก
        foreach (var slot in slots)
        {
            if (slot.isLocked)
            {
                slot.SetItem(item, 1);
                return;
            }
        }

        Debug.Log("Inventory เต็มแล้ว!");
    }
}