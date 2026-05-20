using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraMover : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 2f;
    private bool isMoving = false;

    public void MoveToMap(Transform destination)
    {
        if (!isMoving)
            StartCoroutine(FadeAndMove(destination));
    }

    IEnumerator FadeAndMove(Transform destination)
    {
        isMoving = true;

        // เฟดดำ
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // ย้ายกล้อง
        transform.position = new Vector3(
            destination.position.x,
            transform.position.y,
            transform.position.z
        );

        // เฟดกลับ
        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        isMoving = false;
    }
}

