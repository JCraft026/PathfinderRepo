using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class BarricadeController : NetworkBehaviour
{
    TrapperAbility trapperAbility;
     GameObject trapper,
                engineer,
                chaser,
                runner;
    int hitCount = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        trapper  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        chaser   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        runner   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        
        trapperAbility = trapper.GetComponent<TrapperAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Utilities.GetDistanceBetweenObjects(trapper.transform.position, gameObject.transform.position) < 2.0){
            Debug.Log("Trapper close to wall");
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && trapper.GetComponent<ManageActiveCharacters>().guardId == trapper.GetComponent<ManageActiveCharacters>().activeGuardId)){
                trapperAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        if(Utilities.GetDistanceBetweenObjects(engineer.transform.position, gameObject.transform.position) < 2.0){
            Debug.Log("The engineer is here");
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && engineer.GetComponent<ManageActiveCharacters>().guardId == engineer.GetComponent<ManageActiveCharacters>().activeGuardId)){
                trapperAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        if(Utilities.GetDistanceBetweenObjects(chaser.transform.position, gameObject.transform.position) < 2.0){
            Debug.Log("Chaser is close to wall");
            if((Input.GetKeyDown("e") && CustomNetworkManager.isRunner == false && chaser.GetComponent<ManageActiveCharacters>().guardId == chaser.GetComponent<ManageActiveCharacters>().activeGuardId)){
                trapperAbility.decreseBarricadeCount();
                destroyBarricade();
            }
        }
        if(Utilities.GetDistanceBetweenObjects(runner.transform.position, gameObject.transform.position) < 2.0){
            Debug.Log("Runner is close to wall");
            if(Input.GetKeyDown("e") && CustomNetworkManager.isRunner == true){
                hitCount += 1;
                if(hitCount >= 3){
                    trapperAbility.decreseBarricadeCount();
                    destroyBarricade();
                    hitCount = 0;
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void destroyBarricade(){
        NetworkServer.Destroy(gameObject);
    }
}
