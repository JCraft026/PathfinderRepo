using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;

public class BarricadeController : NetworkBehaviour
{
    EngineerAbility engineerAbility;
     GameObject trapper,
                engineer,
                chaser,
                runner;
    int hitCount = 0;
    bool trapperTooltip = false,
         engineerTooltip = false,
         chaserTooltip = false,
         runnerTooltip = false;   //
    
    // Start is called before the first frame update
    void Awake()
    {
        trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        runner   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        
        engineerAbility = engineer.GetComponent<EngineerAbility>();
    }

    // Update is called once per frame
    void Update()
    {
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

    [Command(requiresAuthority = false)]
    public void destroyBarricade(){
        NetworkServer.Destroy(gameObject);
    }

    void enableTooltip(){
        transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log("enabled tooltip");
    }

    void disableTooltip(){
        transform.GetChild(0).gameObject.SetActive(false);
        Debug.Log("disabled tooltip");
    }
}
