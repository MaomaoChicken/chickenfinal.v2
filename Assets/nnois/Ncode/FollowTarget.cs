using UnityEngine;
using TMPro;

public class FollowTarget : MonoBehaviour
{
    public Transform target;          // drag Trash object here
    public Vector3 offset = new Vector3(0f, 1.5f, 0f); // above the box
    public float smoothSpeed = 8f;    // higher = snappier, lower = floatier

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}