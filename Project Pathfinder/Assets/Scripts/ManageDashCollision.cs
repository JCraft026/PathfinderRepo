using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageDashCollision : MonoBehaviour
{
    public CameraShake cameraShake;   // Holds the camera shaker script
    public GameObject chaser;         // GameObject of the chaser
    public bool attackLanded = false; // Status of dash attack being landed
    
    void Update(){
        // Reset attack landed status
        if(attackLanded && !Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<Animator>().GetBool("Dashing")){
            attackLanded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        Regex chaserExpression = new Regex("Chaser");
        int activeGuardId = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<ManageActiveCharacters>().activeGuardId;
        chaser = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
        
        Debug.Log("Woop");

        if(chaserExpression.IsMatch(collision.gameObject.name) && chaser.GetComponent<Animator>().GetBool("Dashing") && attackLanded == false){
            gameObject.GetComponent<ManageRunnerStats>().TakeDamage(2);
            attackLanded = true;
            if(CustomNetworkManager.isRunner){
                cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(R)")).transform.GetChild(0).GetComponent<CameraShake>();
            }
            else{
                if(activeGuardId == ManageActiveCharactersConstants.CHASER){
                    cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(C)")).transform.GetChild(0).GetComponent<CameraShake>();
                }
            }
            StartCoroutine(cameraShake.Shake(.15f, .7f));
        }
    }
}
