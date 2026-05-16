using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Prefab")]
    public GameObject npcPrefab;

    [Header("ข้อมูลตัวละคร")]
    public DATAChar[] allCharacters;

    [Header("จำนวน NPC สูงสุดในฉาก")]
    public int maxNPC = 5;

    [Header("เวลา Spawn")]
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    [Header("ตำแหน่ง Y ที่ Spawn")]
    public float spawnY = -2f;

    [Header("ระยะนอกจอที่ Spawn")]
    public float spawnOffsetX = 1.5f;

    [Header("Sorting Layer")]
    public string sortingLayerName = "Default";

    private List<GameObject> spawnedNPCs = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            spawnedNPCs.RemoveAll(npc => npc == null);

            if (spawnedNPCs.Count < maxNPC)
                SpawnSingleNPC();

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnSingleNPC()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("[NPCSpawner] ยังไม่ได้ใส่ npcPrefab!");
            return;
        }

        DATAChar data = GetUnusedCharacter();
        if (data == null) return;

        bool fromLeft = Random.value > 0.5f;
        float screenEdgeX = Camera.main.orthographicSize * Camera.main.aspect;
        float spawnX = fromLeft
            ? -screenEdgeX - spawnOffsetX
            : screenEdgeX + spawnOffsetX;

        GameObject npc = Instantiate(npcPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
        npc.name = $"NPC_{Random.Range(100, 999)}";

        var walker = npc.GetComponent<NpcRandom>();
        if (walker != null)
        {
            walker.characterData = data;
            npc.GetComponent<SpriteRenderer>().sprite = data.idleSprite;

            float targetX = Random.Range(-screenEdgeX + 1f, screenEdgeX - 1f);
            walker.targetPosition = new Vector2(targetX, spawnY);
        }

        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            npc.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
            npc.GetComponent<SpriteRenderer>().sortingOrder = spawnedNPCs.Count;
        }

        spawnedNPCs.Add(npc);
        Debug.Log($"[NPCSpawner] Spawn {npc.name} ({data.name}) จาก{(fromLeft ? "ซ้าย" : "ขวา")} | ในฉาก: {spawnedNPCs.Count}/{maxNPC}");
    }

    DATAChar GetUnusedCharacter()
    {
        if (allCharacters == null || allCharacters.Length == 0) return null;

        List<DATAChar> usedData = new List<DATAChar>();
        foreach (var npc in spawnedNPCs)
        {
            if (npc != null)
            {
                var walker = npc.GetComponent<NpcRandom>();
                if (walker != null && walker.characterData != null)
                    usedData.Add(walker.characterData);
            }
        }

        List<DATAChar> available = new List<DATAChar>();
        foreach (var data in allCharacters)
        {
            if (!usedData.Contains(data))
                available.Add(data);
        }

        if (available.Count == 0)
            return allCharacters[Random.Range(0, allCharacters.Length)];

        return available[Random.Range(0, available.Count)];
    }

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        float edgeX = Camera.main.orthographicSize * Camera.main.aspect;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-edgeX - spawnOffsetX, spawnY - 0.3f), new Vector3(-edgeX - spawnOffsetX, spawnY + 0.3f));
        Gizmos.DrawLine(new Vector3(edgeX + spawnOffsetX, spawnY - 0.3f), new Vector3(edgeX + spawnOffsetX, spawnY + 0.3f));
    }
}