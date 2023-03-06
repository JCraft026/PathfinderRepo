using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RenderSmokeScreen : NetworkBehaviour
{
    public static RenderSmokeScreen Instance; // Makes an instance of this class to access attribtuess
    MoveCharacter runnerScript;

    // Called on Start
    void Start(){
        Instance = this;
        runnerScript = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).GetComponent<MoveCharacter>();    
        }
    // Update is called once per frame
    [Command(requiresAuthority = false)]
    public void useSmoke(){
        GameObject gameObject = Instantiate(ItemAssets.Instance.SmokeScreen,
           runnerScript.rigidBody.position, Quaternion.identity);
        NetworkServer.Spawn(gameObject);
    }

    void Awake(){
        if(gameObject != null){
            Destroy(gameObject, 10.0f);
        }
    }
}