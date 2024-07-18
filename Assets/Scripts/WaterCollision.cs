using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WaterCollision : MonoBehaviour
{
    public TextMeshProUGUI messageText; // Reference to the UI Text element
    public string message = "Click <Space>";
    private bool collidingWithWater = false; 

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false); // Hide the message initially
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
            }
            collidingWithWater = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
            collidingWithWater = false;
        }
    }

    void Update()
    {
        if (collidingWithWater && Input.GetKeyDown(KeyCode.Space)) 
        {
            SceneManager.LoadScene("Scene2");
        }
    }
}
