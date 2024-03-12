using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageGoMenuClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        // Allow collider trigger interaction with queries
        Physics2D.queriesHitTriggers = true;
    }

    void OnMouseDown()
    {
        // Trigger exit game event
        gameObject.GetComponent<PauseGame>().ExitGame(0);
    }

    void OnMouseOver(){
        GameObject.Find("Main Menu Button Text").GetComponent<TextMeshPro>().color = new Color(0, 5, 0);
    }

    void OnMouseExit(){
        GameObject.Find("Main Menu Button Text").GetComponent<TextMeshPro>().color = Color.black;
    }
}
