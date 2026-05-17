using System.Collections;
using UnityEngine;

public class NpcRandom : MonoBehaviour
{
    [Header("การตั้งค่าการเดิน")]
    public float walkSpeed = 2f;
    public float minWalkDistance = 1f;
    public float maxWalkDistance = 5f;

    [Header("เวลาหยุด")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 4f;

    [Header("ขอบเขตการเดิน (World Space)")]
    public Vector2 boundsMin = new Vector2(-5f, -3f);
    public Vector2 boundsMax = new Vector2(5f, 3f);

    [Header("ข้อมูลตัวละคร")]
    public DATAChar characterData;

    public string walkingParmeter = "isWalking";
    public bool showGizmos = true;

    [HideInInspector] public Vector2 targetPosition;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isWalking = false;
    private bool hasAnimator = false;
    private Coroutine walkAnimCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hasAnimator = animator != null;
    }

    void Start()
    {
        if (characterData != null && characterData.idleSprite != null)
            spriteRenderer.sprite = characterData.idleSprite;

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
        float randomX = (Random.value > 0.5f) ? distance : -distance;
        float clampedX = Mathf.Clamp(transform.position.x + randomX, boundsMin.x, boundsMax.x);
        return new Vector2(clampedX, transform.position.y);
    }

    IEnumerator MoveToTarget()
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.05f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;

            Vector2 newPos = transform.position;
            newPos.x = Mathf.MoveTowards(newPos.x, targetPosition.x, walkSpeed * Time.deltaTime);
            transform.position = newPos;

            yield return null;
        }
        transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
    }

    void SetWalking(bool walking)
    {
        isWalking = walking;

        if (walking)
        {
            if (walkAnimCoroutine != null) StopCoroutine(walkAnimCoroutine);
            walkAnimCoroutine = StartCoroutine(WalkAnimRoutine());
        }
        else
        {
            if (walkAnimCoroutine != null)
            {
                StopCoroutine(walkAnimCoroutine);
                walkAnimCoroutine = null;
            }
            if (characterData != null && characterData.idleSprite != null)
                spriteRenderer.sprite = characterData.idleSprite;
        }

        if (hasAnimator && !string.IsNullOrEmpty(walkingParmeter))
            animator.SetBool(walkingParmeter, walking);
    }

    IEnumerator WalkAnimRoutine()
    {
        if (characterData == null || characterData.walkSprites == null || characterData.walkSprites.Length < 2)
            yield break;

        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = characterData.walkSprites[index % characterData.walkSprites.Length];
            index++;
            yield return new WaitForSeconds(characterData.walkAnimSpeed);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) / 2f, (boundsMin.y + boundsMax.y) / 2f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}