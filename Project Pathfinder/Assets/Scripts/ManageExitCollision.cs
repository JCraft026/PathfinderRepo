using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ManageExitCollision : MonoBehaviour
{
    // Manage maze exit collisions
    void OnCollisionEnter2D(Collision2D collision){
        Regex runnerExpression = new Regex("Runner");
        
        if(runnerExpression.IsMatch(collision.gameObject.name)){
            HandleEvents.currentEvent = HandleEventsConstants.RUNNER_WINS;
        }
    }
}
