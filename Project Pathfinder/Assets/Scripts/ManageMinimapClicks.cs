using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageMinimapClicks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesHitTriggers = true;
    }

    // Initiate guard fast travel if the minimap floor parent object is clicked
    void OnMouseDown()
    {
        if(!CustomNetworkManager.isRunner)
        {
           Debug.Log(gameObject.name); 
        }
    }
}
