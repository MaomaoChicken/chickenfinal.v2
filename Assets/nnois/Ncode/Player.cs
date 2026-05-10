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
   
    void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }
    public void Flip(float side)
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= side;
        transform.localScale = localScale;
    }
}
