using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ManageEnterTunnelCollision : MonoBehaviour
{
    // Manage enter tunnel collisions
    void OnCollisionEnter2D(Collision2D collision){
        Regex runnerExpression = new Regex("Runner");
        
        if(runnerExpression.IsMatch(collision.gameObject.name)){
            Debug.Log("Runner Enter Tunnel");
        }
    }
}
