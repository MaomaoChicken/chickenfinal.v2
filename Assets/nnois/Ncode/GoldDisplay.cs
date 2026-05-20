using UnityEngine;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class GoldDisplay : MonoBehaviour
{
    private TextMeshProUGUI txt;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (PlayerInventory.Instance != null)
            txt.text = $"Gold: {PlayerInventory.Instance.gold}";
    }
}