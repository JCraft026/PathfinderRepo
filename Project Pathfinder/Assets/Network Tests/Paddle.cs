using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Paddle : NetworkBehaviour
{
    public bool isPlayer1;
    public float speed; //Limit of how fast the player can move
    public Rigidbody2D rb; //Reference to the rigid body

    private float movement; //Determines if we are moving up or down

    // Update is called once per frame
    void Update()
    {
        if(isPlayer1)
        {
            movement = Input.GetAxisRaw("Vertical"); //References player 1
        }
        else
        {
            //movement = Input.GetAxisRaw("Vertical2"); //references player 2 (DEBUG: NOTE: set this up for networking)
        }

        rb.velocity = new Vector2(rb.velocity.x, movement * speed);

    }
}
