using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageExitCollision : MonoBehaviour
{
    // Manage maze exit collisions
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.name == "Runner"){
            HandleEvents.currentEvent = HandleEventsConstants.RUNNER_WINS;
        }
    }
}
