using UnityEngine;

public class TrashDrop : MonoBehaviour
{
    public float pickupRange = 0.5f;
    public bool isBeingPickedUp = false;

    [Header("Item drop to ItemData")]
    public ItemData itemData;
    void OnMouseDown()
    {
       if (isBeingPickedUp) return;
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            isBeingPickedUp = true;
            player.CollectItem(this);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
