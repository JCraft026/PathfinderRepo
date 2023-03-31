using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageRunnerStats : MonoBehaviour
{
    public int health = 6;           // Runner health
    public GameObject heart1;        // First HP heart
    public GameObject heart2;        // Second HP heart
    public GameObject heart3;        // Third HP heart
    public Animator animator;        // Character's animator manager
    public GameObject damageSoundsObject;
                                     // GameObject that damageSounds comes from
    public AudioSource damageSounds; // Characters audiosource for taking damage

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

    void Awake() 
    {
        damageSounds = damageSoundsObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update UI hearts
        if(health <= 4){
            if(CustomNetworkManager.isRunner){
                heart3.SetActive(false);
            }
        }
        if(health <= 2){
            if(CustomNetworkManager.isRunner){
                heart2.SetActive(false);
            }
        }
        if(health <= 0){
            if(CustomNetworkManager.isRunner){
                heart1.SetActive(false);
            }
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
        damageSounds.Play();
        Debug.Log("The runner took: " + damage.ToString() + " damage");
    }
}
