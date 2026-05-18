using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "ShopSystem/ShopItem")]
public class ShopItemData : ScriptableObject
    {
        [Header("อ้างอิง ItemData เดิม")]
        public ItemData itemData;          // ลาก ItemData.cs เดิมมาใส่ตรงนี้

        [Header("ราคา")]
        public int basePrice = 100;

        [Range(0.1f, 1f)]
        public float minPriceMultiplier = 0.5f;   // ราคาต่ำสุดที่ต่อได้ (50% ของ base)

        [Range(1f, 3f)]
        public float maxPriceMultiplier = 1.5f;   // ราคาสูงสุดที่ต่อได้ (150% ของ base)

        [Range(0, 100)]
        public int merchantStubborn = 50;          // ความดื้อพ่อค้า 0=ยอมง่าย 100=ดื้อมาก

        // Shortcut — ดึงข้อมูลจาก ItemData 
        public string ItemName => itemData != null ? itemData.itemName : name;
        public Sprite Icon => itemData != null ? itemData.icon : null;
    }
