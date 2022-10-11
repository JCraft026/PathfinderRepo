using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GModeSquareMovement : NetworkBehaviour
{
    public float moveSpeed;         //Determines how fast the square will go
    public Rigidbody2D rigidBody;   //The RigidBody2D object
    private Vector2 moveDirection;  //Determines the direction and speed the square should go
    public bool isTangible;         //Determines whether or not the square can phase through walls (Not Implemented)
    

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
            ProcessInputs();
    }

    //Similar to Update() but is called more consistently apparently
    private void FixedUpdate()
    {
        Move();
    }

    //Process GModeSquare inputs (uses standard WASD control scheme)
    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    //Calculates the movement vector
    void Move()
    {
        rigidBody.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }
}
