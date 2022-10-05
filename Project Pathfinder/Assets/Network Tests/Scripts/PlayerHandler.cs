using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class PlayerHandler : Mirror.NetworkBehaviour
{
    public bool isPlayer1;
    public float speed; //Limit of how fast the player can move
    public Rigidbody2D rb; //Reference to the rigid body

    private float movement; //Determines if we are moving up or down

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isLocalPlayer)
        {
            movement = Input.GetAxisRaw("Vertical"); //References player 1
            rb.velocity = new Vector2(rb.velocity.x, movement * speed);
        }

    }
}
