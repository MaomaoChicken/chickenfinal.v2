using UnityEngine;

public class TrashDrop : MonoBehaviour
{
    Vector3 mousePositionoffset;

    private Vector3 GetMouseWorldProsition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        mousePositionoffset = gameObject.transform.position - GetMouseWorldProsition();
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldProsition() + mousePositionoffset;
    }
}
