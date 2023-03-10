using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageGoMenuClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Allow collider trigger interaction with queries
        Physics2D.queriesHitTriggers = true;
    }

    void OnMouseDown()
    {
        // Trigger exit game event
        gameObject.GetComponent<PauseGame>().ExitGame(0);
    }
}
