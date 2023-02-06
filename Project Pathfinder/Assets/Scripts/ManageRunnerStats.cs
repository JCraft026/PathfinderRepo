using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageRunnerStats : MonoBehaviour
{
    public int health = 6; // Runner health

    // Start is called before the first frame update
    void Start()
    {
        if(CustomNetworkManager.isRunner){
            GameObject.Find("Heart1").SetActive(true);
            GameObject.Find("Heart2").SetActive(true);
            GameObject.Find("Heart3").SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 4){
            GameObject.Find("Heart3").SetActive(false);
        }
        if(health <= 2){
            GameObject.Find("Heart3").SetActive(false);
        }
        if(health <= 0){
            GameObject.Find("Heart3").SetActive(false);
            HandleEvents.currentEvent = HandleEventsConstants.GUARDMASTER_WINS;
        }
    }

    public void TakeDamage(int damage){
        health -= damage;
        // If character is runner: Check health amount and adjust hearts accordingly
    }
}
