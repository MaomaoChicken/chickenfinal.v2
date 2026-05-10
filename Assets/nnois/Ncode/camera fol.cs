using UnityEngine;

public class camerafol : MonoBehaviour
{
    public Vector3 offset;
    public float damping;

    public Transform target;
    public Vector3 vel = Vector3.zero;

    public void FixedUpdate()
    {
         Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(target.position,targetPosition, ref vel, damping);
    }
}
