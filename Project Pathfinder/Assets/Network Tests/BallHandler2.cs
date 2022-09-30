using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler2 : MonoBehaviour
{
    public float speed; //Limit of how fast the ball can move
    public Rigidbody2D rb; //Reference to the rigid body

    void Start()
    {
        Launch();
    }

    private void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;

        rb.velocity = new Vector2(speed * x, speed * y);
    }
}
