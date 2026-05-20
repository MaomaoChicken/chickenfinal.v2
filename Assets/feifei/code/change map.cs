using Microsoft.Unity.VisualStudio.Editor;
using System.Data;
using UnityEngine;
using static UnityEditor.SceneView;

public class changemap : MonoBehaviour
{
    public CameraMover cameraMover;
    public Transform destination;
    public GameObject pressKeyUI; // UI บอกให้กด เช่น "กด E เพื่อไปต่อ"

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            cameraMover.MoveToMap(destination);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (pressKeyUI != null)
                pressKeyUI.SetActive(true); // แสดง UI
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (pressKeyUI != null)
                pressKeyUI.SetActive(false); // ซ่อน UI
        }
    }
}


    