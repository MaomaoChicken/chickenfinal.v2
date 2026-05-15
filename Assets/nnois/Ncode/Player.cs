using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public float moveInput;
    public bool isFacingRight;
    [SerializeField] Sprite[] sprite;
    public float moveToItemSpeed = 4f;
    public float collectDuration = 0.5f;
    public bool isCollecting = false;
   
    void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isCollecting) return;
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput == 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprite[0];
            
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprite[1];
            GetComponent<SpriteRenderer>().flipX = moveInput < 0;
        }

    }

    void FixedUpdate()
    {
        if (isCollecting) return;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }
    public void Flip(float side)
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= side;
        transform.localScale = localScale;
    }
    public void CollectItem(TrashDrop item)
    {
        if (isCollecting) return;
        StartCoroutine(CollectRoutine(item));
    }
    IEnumerator CollectRoutine(TrashDrop item)
    {
        isCollecting = true;
        while (item != null)
        {
            float dist = Vector2.Distance(transform.position, item.transform.position);
            Debug.Log($"ระยะ: {dist} | pickupRange: {item.pickupRange}");
            if (dist <= item.pickupRange)
            {
                Debug.Log("ถึงระยะแล้ว!");
                break;
            }
            Vector2 dir = (item.transform.position - transform.position).normalized;
            GetComponent<SpriteRenderer>().sprite = sprite[1];
            GetComponent<SpriteRenderer>().flipX = dir.x < 0;
            rb.linearVelocity = new Vector2(dir.x * moveToItemSpeed, rb.linearVelocityY);
            yield return null;
        }

        rb.linearVelocity = new Vector2 (0, rb.linearVelocityY);
        if (sprite[2] != null)
            GetComponent<SpriteRenderer>().sprite = sprite[2];
        if (item != null)
        {
                Debug.Log($"[Player] เก็บ {item.gameObject.name}แล้ว");
                item.isBeingPickedUp = false;
                Destroy(item.gameObject);
        }
        yield return new WaitForSeconds(collectDuration);
        GetComponent<SpriteRenderer>().sprite = sprite[0];
        isCollecting = false;

    }
}
