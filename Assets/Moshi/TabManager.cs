using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject allInventory;   // ลาก AllInventory
    public GameObject allCrafting;    // ลาก AllCrafting

    public Button inventoryButton;    // ลาก Button Inventory
    public Button craftButton;        // ลาก Button Crafting

    void Start()
    {
        inventoryButton.onClick.AddListener(() => ShowTab("inventory"));
        craftButton.onClick.AddListener(() => ShowTab("craft"));

        ShowTab("inventory");
    }

    public void ShowTab(string tab)
    {
        allInventory.SetActive(tab == "inventory");
        allCrafting.SetActive(tab == "craft");
    }
}