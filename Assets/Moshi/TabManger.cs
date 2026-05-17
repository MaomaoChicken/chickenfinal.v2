using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject mainInventory;
    public GameObject mainCrafting;

    public Button invButtonInInventory;   // Button Inventory ใน MainInventory
    public Button craftButtonInInventory; // Button Crafting ใน MainInventory
    public Button invButtonInCrafting;    // Button Inventory ใน MainCrafting
    public Button craftButtonInCrafting;  // Button Crafting ใน MainCrafting

    void Start()
    {
        invButtonInInventory.onClick.AddListener(() => ShowTab("inventory"));
        craftButtonInInventory.onClick.AddListener(() => ShowTab("crafting"));
        invButtonInCrafting.onClick.AddListener(() => ShowTab("inventory"));
        craftButtonInCrafting.onClick.AddListener(() => ShowTab("crafting"));
    }

    public void ShowTab(string tab)
    {
        mainInventory.SetActive(tab == "inventory");
        mainCrafting.SetActive(tab == "crafting");
    }
}