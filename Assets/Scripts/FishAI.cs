using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    public float patrolSpeed = 2f; // Speed of patrolling
    public float playerDetectionRadius = 5f; // Distance at which the enemy detects the player
    public float detectionRange = 3f;
    public float minPatrolDistance = 1f; // Distance the enemy moves left and right while patrolling
    public float maxPatrolDistance = 5f;
    public LayerMask playerLayer; // Layer of the player
    public LayerMask wallLayer;

    private Vector2 initialPosition;
    private Vector2 currentDirection;
    private Rigidbody2D rb;
    private bool movingRight = false; // Start moving to the right
    private float patrolRightBoundary;
    private float patrolLeftBoundary;
    public float normalGravity = 3f;       // Gravity when not in water
    public float waterGravity = 0f;      // Gravity when in water
    private float health = 2f;
    public float immunityDuration = .5f; // Duration of immunity frames in seconds
    private bool isImmune = false; // Indicates if the object is currently immune

    public float dashSpeed = 5f; // Speed during dash
    public float dashDuration = 0.1f; // Duration of dash
    public float dashCooldown = 0.5f; // Cooldown between dashes

    private bool isDashing = false;
    private float dashCooldownTime;
    private float dashTime;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        SetPatrolBoundaries();
    }

    void SetPatrolBoundaries()
    {
        patrolRightBoundary = initialPosition.x + Random.Range(minPatrolDistance, maxPatrolDistance);
        patrolLeftBoundary = initialPosition.x - Random.Range(minPatrolDistance, maxPatrolDistance);
    }

    void Patrol()
    {
        if (movingRight)
        {
            currentDirection = Vector2.right;
            if (transform.position.x >= patrolRightBoundary)
            {
                movingRight = false;
                // Flip();
            }
        }
        else
        {
            currentDirection = Vector2.left;
            if (transform.position.x <= patrolLeftBoundary)
            {
                movingRight = true;
                // Flip();
            }
        }
    }

    void InnerCircle(bool wall, bool playerInner)
    {
        if(!isDashing) {
            if (wall && !playerInner)
            {
                List<Collider2D> detectedColliders = DetectCollidersInRange(transform.position, detectionRange, wallLayer, "Wall");
                Vector2 direction = new Vector2(0, 0);
                RaycastHit2D hitleft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, wallLayer);
                RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, wallLayer);
                RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, detectionRange, wallLayer);
                RaycastHit2D hitdown = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, wallLayer);
                if(hitleft.collider != null) {
                    direction = direction + Vector2.right;
                } else if (hitright.collider != null) {
                    direction = direction + Vector2.left;
                }
                if(hitup.collider != null) {
                    direction = direction + Vector2.down;
                } else if (hitdown.collider != null) {
                    direction = direction + Vector2.up;
                }
                currentDirection = direction.normalized;
            }
            else if (wall && IsPlayerWithinOuter() && !isDashing)
            {
                List<Collider2D> detectedColliders = DetectCollidersInRange(transform.position, detectionRange, wallLayer, "Wall");
                Vector2 direction = new Vector2(0, 0);
                RaycastHit2D hitleft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, wallLayer);
                RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, wallLayer);
                RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, detectionRange, wallLayer);
                RaycastHit2D hitdown = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, wallLayer);
                if(hitleft.collider != null) {
                    direction = direction + Vector2.right;
                } else if (hitright.collider != null) {
                    direction = direction + Vector2.left;
                }
                if(hitup.collider != null) {
                    direction = direction + Vector2.down;
                } else if (hitdown.collider != null) {
                    direction = direction + Vector2.up;
                }
                Vector2 normalToWall = direction.normalized;
                Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                Vector3 playerToFish = (transform.position - playerPosition).normalized;
                Vector2 perp1 = new Vector2(playerToFish.y, -playerToFish.x);
                Vector2 perp2 = new Vector2(-playerToFish.y, playerToFish.x);
                if(Vector2.Dot(perp1, normalToWall) > 0) {
                    direction = perp1;
                } else {
                    direction = perp2;
                }
                currentDirection = direction.normalized;
            }
            else if (!wall && playerInner)
            {
                Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                currentDirection = (transform.position - playerPosition).normalized;
            }
        }
    }

    bool IsWallWithinInner()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, wallLayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    List<Collider2D> DetectCollidersInRange(Vector2 position, float range, LayerMask layerMask, string tag)
    {
        List<Collider2D> collidersList = new List<Collider2D>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range, layerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                collidersList.Add(collider);
            }
        }

        return collidersList;
    }

    bool IsPlayerWithinInner()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPlayerWithinOuter()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, playerDetectionRadius, playerLayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // void Flip()
    // {
    //     Vector3 theScale = transform.localScale;
    //     theScale.x *= -1;
    //     transform.localScale = theScale;
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !isImmune)
        {
            TakeDamage();
            if (health <= 0)
            {
                rb.gravityScale = -2f;
            }
        }
        if (collision.gameObject.CompareTag("Player") && health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void TakeDamage()
    {
        health--;
        StartCoroutine(ImmunityFrames());
    }

    IEnumerator ImmunityFrames()
    {
        isImmune = true;
        yield return new WaitForSeconds(immunityDuration);
        isImmune = false;
    }

    public static Vector3 rotate(Vector2 v, float delta)
    {
        return new Vector3(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta), 0
        );
    }

    public void Update()
    {
        if (health > 0)
        {
            bool playerInner = IsPlayerWithinInner();
            bool wall = IsWallWithinInner();
            float currentSpeed = patrolSpeed;
            if (isDashing)
            {
                if(Time.time >= dashTime) {
                    isDashing = false;
                } else {
                    currentSpeed = dashSpeed;
                }
            } 
            if (playerInner || wall)
            {
                InnerCircle(wall, playerInner);
                if(!isDashing && Time.time >= dashCooldownTime)
                {
                    StartDash();
                    currentSpeed = dashSpeed;
                }   
            }
            else if (!IsPlayerWithinOuter())
            {
                Patrol();
                currentSpeed = patrolSpeed;
            }
            Debug.DrawLine(transform.position, transform.position + (Vector3)currentDirection * 2, Color.green);

            rb.MovePosition(rb.position + currentDirection * currentSpeed * Time.fixedDeltaTime);
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
}
