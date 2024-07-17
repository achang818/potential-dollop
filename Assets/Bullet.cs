using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool hasBeenCollected = false;
    private Rigidbody2D rb;
    private bool collided = false;
    private Transform stuckToMob;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (collided && stuckToMob != null)
        {
            // Ensure the spear keeps its initial rotation
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }
    }

    void FixedUpdate()
    {
        if(!collided) {
            // Get the current velocity
            Vector2 velocity = rb.velocity;

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 45f;

            // Set the rotation of the bullet
            rb.rotation = angle;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mob") && !collided)
        {
            // Stick the spear to the mob
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Make the spear stop moving
            transform.parent = collision.transform; // Make the spear a child of the mob
            stuckToMob = collision.transform;
            collided = true;
            return;
        }
        if(!collision.gameObject.CompareTag("Bullet")) {
            collided = true;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation upon collision
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            transform.parent = collision.transform;
            
            // Check if the collision is with the player and if the ammo has not already been collected
            if (collision.gameObject.CompareTag("Player") && !hasBeenCollected)
            {
                PlayerShooting playerShooting = collision.gameObject.GetComponent<PlayerShooting>();
                if (playerShooting != null)
                {
                    playerShooting.ammo++;
                    hasBeenCollected = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
