using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject mainInventoryGroup;
    public TabManager tabManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = mainInventoryGroup.activeSelf;
            mainInventoryGroup.SetActive(!isOpen);

            if (!isOpen)
                tabManager.ShowTab("inventory");
        }
    }
}