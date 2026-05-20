using Microsoft.Unity.VisualStudio.Editor;
using System.Data;
using UnityEngine;
using static UnityEditor.SceneView;

public class changemap : MonoBehaviour
{
    public CameraMover cameraMover;  // ลาก Main Camera มาใส่
    public Transform m13;
    public Transform m21;
    public Transform m22;
    public Transform m31;
    public Transform m32;
    public Transform m41;
    public Transform destination;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ย้ายกล้องจาก m13 ไป m21
            cameraMover.MoveToMap(destination);
        }
    }
}


    