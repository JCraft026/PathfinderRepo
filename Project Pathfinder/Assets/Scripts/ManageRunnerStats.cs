using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ManageRunnerStats : MonoBehaviour
{
    public int health = 6;         // Runner health
    public GameObject heart1;      // First HP heart
    public GameObject heart2;      // Second HP heart
    public GameObject heart3;      // Third HP heart
    public Animator animator;      // Character's animator manager
    public int currentHeartId = 3; // ID of the current heart being manipulated
    public Sprite halfHeart;        // Half heart image for runner UI

    // Start is called before the first frame update
    void Start()
    {
        // Spawn UI hearts
        if(CustomNetworkManager.isRunner){
            heart1 = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart1"));
            heart2 = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart2"));
            heart3 = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3"));

            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Trigger GuardMaster wins event if runner is defeated
        if(health <= 0){
            HandleEvents.currentEvent = HandleEventsConstants.GUARDMASTER_WINS;
        }

        // Trigger unlock exit attempt
        if(Input.GetKeyDown("space") && CustomNetworkManager.isRunner){
            animator.SetBool("Unlock Attempted", true);
        }
    }

    // Process runner damage
    public void TakeDamage(int damage){
        health -= damage;
        switch (health)
        {
            case 5:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).GetComponent<Image>().sprite = halfHeart;
                break;
            case 4:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).SetActive(false);
                break;
            case 3:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart2")).GetComponent<Image>().sprite = halfHeart;
                break;
            case 2:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart2")).SetActive(false);
                break;
            case 1:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart2")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart1")).GetComponent<Image>().sprite = halfHeart;
                break;
            case 0:
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart3")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart2")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Heart1")).SetActive(false);
                break;
        }
    }
}
