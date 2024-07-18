using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public CharacterController2D controller; 
    float move = 0f;
    public float moveSpeed;
    bool jump = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxisRaw("Horizontal") * moveSpeed;
        if(Input.GetButtonDown("Jump")) {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        controller.Move(move * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
