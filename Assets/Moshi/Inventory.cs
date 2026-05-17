using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject mainInventory;
    public GameObject mainCrafting;
    public TabManager tabManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = mainInventory.activeSelf || mainCrafting.activeSelf;

            if (isOpen)
            {
                mainInventory.SetActive(false);
                mainCrafting.SetActive(false);
            }
            else
            {
                tabManager.ShowTab("inventory");
            }
        }
    }
}