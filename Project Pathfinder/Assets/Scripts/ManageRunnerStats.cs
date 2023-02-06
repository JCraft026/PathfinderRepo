using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageRunnerStats : MonoBehaviour
{
    public int health = 6; // Runner health

    // Start is called before the first frame update
    void Start()
    {
        // If character is runner: Initialize health hearts on canvas
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0){
            Debug.Log("Runner dead");
            // Call Runner dead event
        }
    }

    public void TakeDamage(int damage){
        health -= damage;
        // If character is runner: Check health amount and adjust hearts accordingly
    }
}
