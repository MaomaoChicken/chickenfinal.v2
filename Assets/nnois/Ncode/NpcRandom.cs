using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class NpcRandom : MonoBehaviour
{
    [Header("การตั้งค่าการเดิน")]
    [Tooltip("ความเร็วในการเดิน")]
    public float walkSpeed = 2f;

    [Tooltip("ระยะเดินขั้นต่ำและสูง")]
    public float minWalkDistance = 1f;
    public float maxWalkDistance = 5f;
    [Tooltip("เวาลาหยุด")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 4f;

    [Header("ขอบเขตการเดิน (World Space)")]
    [Tooltip("ซ้ายล่าง")]
    public Vector2 boundsMin = new Vector2(-5f, -3f);
    [Tooltip("ขวาบน")]
    public Vector2 boundsMax = new Vector2(5f, 3f);

    public Sprite[] CharacterSprites;
    public string walkingParmeter = "isWalking";
    public bool showGizmos = true;
    public Vector2 targetPosition;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public bool isWalking = false;
    public bool hasAnimator = false;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasAnimator = animator != null;
    }
    void Start()
    {
        targetPosition = transform.position;
        StartCoroutine(WalkRoutine());
    }
    IEnumerator WalkRoutine()
    {
        while (true)
        {
            SetWalking(false);
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);
            targetPosition = GetRandomTarget();
            SetWalking(true);
            yield return StartCoroutine(MoveToTarget());
        }
    }
    Vector2 GetRandomTarget()
    {
        float distance = Random.Range(minWalkDistance, maxWalkDistance);
        float randomX = (Random.value > 0.05f) ? distance : -distance;
        float clampedX = Mathf.Clamp(transform.position.x + randomX, boundsMin.x, boundsMax.x);
        return new Vector2(randomX, transform.position.y);
    }
    IEnumerator MoveToTarget()
    {
        while (Vector2.Distance(transform.position, targetPosition)>0.05f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
            Vector2 newPos = transform.position;
            newPos.x = Mathf.MoveTowards(newPos.x, targetPosition.x, walkSpeed * Time.deltaTime);
            transform.position = newPos;
            yield return null;
        } 
        transform.position = targetPosition;
    }
    void SetWalking(bool walking)
    {
        isWalking = walking;
        if (hasAnimator && !string.IsNullOrEmpty(walkingParmeter))
        {
            animator.SetBool(walkingParmeter, walking);
        }
    }
    void OnDrawGizmos()
    {
      if (!showGizmos) return;
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) / 2, (boundsMin.y + boundsMax.y) / 2f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x ,boundsMax.y - boundsMin.y , 0f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, 0.15f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}
