using UnityEngine;

/// <summary>
/// ติดกับ Sprite พ่อค้า — ต้องมี Collider2D ด้วย
/// เมื่อ player อยู่ใกล้แล้วคลิก จะเปิด ShopUI
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MerchantNPC : MonoBehaviour
{
    [Header("Merchant Info")]
    public string merchantName = "ร้านค้า";
    public Sprite merchantPortrait;

    [Header("Interaction")]
    public float interactionRadius = 2f;

    [Header("References")]
    public ShopUI shopUI;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    public bool IsPlayerNearby { get; private set; }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // หา Player จาก Tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;

        if (shopUI == null) shopUI = FindFirstObjectByType<ShopUI>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        IsPlayerNearby = dist <= interactionRadius;

        // Highlight เมื่ออยู่ใกล้
        if (spriteRenderer != null)
            spriteRenderer.color = IsPlayerNearby ? new Color(1f, 1f, 0.6f) : Color.white;
    }

    void OnMouseDown()
    {
        if (!IsPlayerNearby)
        {
            Debug.Log("เข้าใกล้พ่อค้าก่อนนะ!");
            return;
        }
        shopUI?.OpenShop(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}