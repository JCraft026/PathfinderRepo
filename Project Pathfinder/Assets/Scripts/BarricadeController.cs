using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;

public class BarricadeController : NetworkBehaviour
{
    EngineerAbility engineerAbility; // Instance of the engineer ability script
     GameObject trapper,             // Gameobject instance of the trapper
                engineer,            // Gameobject instance of the engineer
                chaser,              // Gameobject instance of the chaser
                runner;              // Gameobject instance of the runner
    int hitCount = 0;                // Total number of hits on barricades attacked by a runner
    bool trapperTooltip = false,     // Whether the tooltip to destroy the barricade is active for the trapper
         engineerTooltip = false,    // Whether the tooltip to destroy the barricade is active for the engineer
         chaserTooltip = false,      // Whether the tooltip to destroy the barricade is active for the chaser
         runnerTooltip = false;      // Whether the tooltip to destroy the barricade is active for the runner
    
    // Start is called before the first frame update
    void Awake()
    {
        // Get all player instances
        trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        runner   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        
        // Get the EngineerAbility script
        engineerAbility = engineer.GetComponent<EngineerAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        // Tool tip for trapper
        if(Utilities.GetDistanceBetweenObjects(trapper.transform.position, gameObject.transform.position) < 2.5){
            enableTooltip();
            trapperTooltip = true;
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && trapper.GetComponent<ManageActiveCharacters>().guardId == trapper.GetComponent<ManageActiveCharacters>().activeGuardId)){
                engineerAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        else if(trapperTooltip && CustomNetworkManager.isRunner == false && trapper.GetComponent<ManageActiveCharacters>().guardId == trapper.GetComponent<ManageActiveCharacters>().activeGuardId){
            disableTooltip();
            trapperTooltip = false;
        }
        // Tool tip for engineer
        if(Utilities.GetDistanceBetweenObjects(engineer.transform.position, gameObject.transform.position) < 2.5){
            enableTooltip();
            engineerTooltip = true;
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && engineer.GetComponent<ManageActiveCharacters>().guardId == engineer.GetComponent<ManageActiveCharacters>().activeGuardId)){
                engineerAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        else if(engineerTooltip && CustomNetworkManager.isRunner == false && engineer.GetComponent<ManageActiveCharacters>().guardId == engineer.GetComponent<ManageActiveCharacters>().activeGuardId){
            disableTooltip();
            engineerTooltip = false;
        }
        // Tool tip for chaser
        if(Utilities.GetDistanceBetweenObjects(chaser.transform.position, gameObject.transform.position) < 2.5){
            enableTooltip();
            chaserTooltip = true;
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && chaser.GetComponent<ManageActiveCharacters>().guardId == chaser.GetComponent<ManageActiveCharacters>().activeGuardId)){
                engineerAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        else if(chaserTooltip && CustomNetworkManager.isRunner == false && chaser.GetComponent<ManageActiveCharacters>().guardId == chaser.GetComponent<ManageActiveCharacters>().activeGuardId){
            disableTooltip();
            chaserTooltip = false;
        }
        // Tool tip for runner
        if(Utilities.GetDistanceBetweenObjects(runner.transform.position, gameObject.transform.position) < 2.5){
            enableTooltip();
            runnerTooltip = true;
            if(Input.GetKeyDown("e") && CustomNetworkManager.isRunner == true){
                hitCount += 1;
                if(hitCount >= 3){
                    engineerAbility.decreseBarricadeCount();
                    destroyBarricade();
                    hitCount = 0;
                }
            }
        }
        else if(runnerTooltip && CustomNetworkManager.isRunner == true){
            disableTooltip();
            runnerTooltip = false;
        }
    }

    // Destroys both host and client barricade
    [Command(requiresAuthority = false)]
    public void destroyBarricade(){
        NetworkServer.Destroy(gameObject);
    }

    // Enable the barricade tooltip
    void enableTooltip(){
        transform.GetChild(0).gameObject.SetActive(true);
    }

    // Disable the barriacde tooltip
    void disableTooltip(){
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
