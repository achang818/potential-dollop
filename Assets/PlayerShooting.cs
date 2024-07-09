using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint; // Assign your fire point object in the Inspector
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public float ammo = 1f;
    Vector2 mousePosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if mouse button is pressed
        if (Input.GetMouseButton(0))
        {
            // Get mouse position in world coordinates
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the positions of all four corners of the sprite
            Vector2 spriteBounds = spriteRenderer.bounds.size;
            Vector2 spriteCenter = spriteRenderer.bounds.center;

            Vector2[] corners = new Vector2[4];
            corners[0] = spriteCenter + new Vector2(-spriteBounds.x / 2, spriteBounds.y / 2); // Top-left
            corners[1] = spriteCenter + new Vector2(spriteBounds.x / 2, spriteBounds.y / 2);  // Top-right
            corners[2] = spriteCenter + new Vector2(-spriteBounds.x / 2, -spriteBounds.y / 2); // Bottom-left
            corners[3] = spriteCenter + new Vector2(spriteBounds.x / 2, -spriteBounds.y / 2);  // Bottom-right

            // Determine the closest corner to the mouse click
            Vector2 closestCorner = corners[0];
            float closestDistance = Vector2.Distance(mousePosition, closestCorner);

            for (int i = 1; i < 4; i++)
            {
                float distance = Vector2.Distance(mousePosition, corners[i]);
                if (distance < closestDistance)
                {
                    closestCorner = corners[i];
                    closestDistance = distance;
                }
            }

            // Set fire point position to the closest corner
            firePoint.position = closestCorner;
        }
        if(Input.GetButtonDown("Fire1") && ammo > 0) {
            Shoot();
        }
    }

    void Shoot() {
        Vector2 lookDir = mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle + 45f));
        Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
        rb2d.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        ammo--;
    }
}
