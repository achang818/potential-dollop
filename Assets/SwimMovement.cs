using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;           // Normal movement speed
    public float deceleration = 5f;        // Rate at which the player slows down
    public float dashSpeed = 10f;          // Speed during the dash
    public float dashDuration = 0.2f;      // Duration of the dash
    public float dashCooldown = 1f;        // Cooldown time for the dash
    public float normalGravity = 3f;       // Gravity when not in water
    public float waterGravity = 0.5f;      // Gravity when in water

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTime;
    private bool collidingWithWater = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Initial check to see if the player starts inside the water
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Water"))
            {
                collidingWithWater = true;
                break;
            }
        }
        UpdateGravityScale();
    }

    void Update()
    {
        HandleInput();
        HandleDash();
        UpdateGravityScale();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        if (!isDashing)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = 0;
            if(collidingWithWater) {
                moveY = Input.GetAxisRaw("Vertical");
            }

            movement = new Vector2(moveX, moveY).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && Time.time >= dashCooldownTime && collidingWithWater)
        {
            StartDash();
        }
    }

    void MovePlayer()
    {
        if (isDashing)
        {
            rb.velocity = movement * dashSpeed;
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, movement * moveSpeed, deceleration * Time.fixedDeltaTime);
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTime = Time.time + dashDuration;
        dashCooldownTime = Time.time + dashCooldown;
    }

    void HandleDash()
    {
        if (isDashing && Time.time >= dashTime)
        {
            isDashing = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            collidingWithWater = true;
            UpdateGravityScale();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            collidingWithWater = false;
            UpdateGravityScale();
        }
    }

    void UpdateGravityScale()
    {
        if (collidingWithWater)
        {
            rb.gravityScale = waterGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }
}
