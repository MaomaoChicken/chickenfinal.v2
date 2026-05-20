using UnityEngine;

[System.Serializable]
public class TrashPrefabData
{
    public ItemData itemData;
    public GameObject prefab;
}

public class TrashCan : MonoBehaviour
{
    [Header("ข้อมูลขยะ + prefab")]
    public TrashPrefabData[] trashDatas;

    [Header("จุดเกิด")]
    public Transform spawnPoint;

    public float jumpForce = 7f;

    void OnMouseDown()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        if (trashDatas.Length <= 0) return;

        // สุ่ม
        TrashPrefabData randomTrash =
            trashDatas[Random.Range(0, trashDatas.Length)];

        if (randomTrash.prefab == null)
        {
            Debug.LogWarning("ไม่มี prefab");
            return;
        }

        // Spawn
        GameObject obj = Instantiate(
            randomTrash.prefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // เด้ง
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 force = new Vector2(
                Random.Range(-2f, 2f),
                jumpForce
            );

            rb.AddForce(force, ForceMode2D.Impulse);
        }

        Debug.Log("สุ่มได้: " + randomTrash.itemData.itemName);
    }
}