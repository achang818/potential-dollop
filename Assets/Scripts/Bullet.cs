using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool hasBeenCollected = false;
    private Rigidbody2D rb;
    private bool collided = false;
    private FixedJoint2D fixedJoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (collided)
        {
            // Ensure the spear keeps its initial rotation
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);

            if (fixedJoint == null || fixedJoint.Equals(null))
            {
                collided = false;
                rb.gravityScale = 3f;
                rb.constraints = RigidbodyConstraints2D.None;
                fixedJoint = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (!collided)
        {
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
        if (!collided)
        {
            if (collision.gameObject.CompareTag("Mob") || !collision.gameObject.CompareTag("Bullet"))
            {
                // Stick the spear to the mob using FixedJoint2D
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; 

                fixedJoint = collision.gameObject.AddComponent<FixedJoint2D>();
                fixedJoint.connectedBody = rb;
                fixedJoint.autoConfigureConnectedAnchor = false;
                fixedJoint.anchor = collision.transform.InverseTransformPoint(collision.contacts[0].point);
                fixedJoint.connectedAnchor = transform.InverseTransformPoint(collision.contacts[0].point);

                collided = true;
            }
        }
        if (collision.gameObject.CompareTag("Player") && !hasBeenCollected)
        {
            PlayerShooting playerShooting = collision.gameObject.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.ammo++;
                hasBeenCollected = true;
                if(fixedJoint != null) {
                    Destroy(fixedJoint);
                }
                Destroy(gameObject);
            }
        }
    }
}
