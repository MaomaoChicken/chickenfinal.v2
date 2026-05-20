using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject mainInventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mainInventory.SetActive(!mainInventory.activeSelf);
        }
    }
}