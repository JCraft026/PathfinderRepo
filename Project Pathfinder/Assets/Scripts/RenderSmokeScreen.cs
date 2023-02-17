using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RenderSmokeScreen : NetworkBehaviour
{
    public static RenderSmokeScreen Instance;          // Makes an instance of this class to access attribtues

    // Called on Start
    void Start(){
        Instance = this;
    }

    // Update is called once per frame
    [Command]
    public void useSmoke(){
        GameObject gameObject = Instantiate(ItemAssets.Instance.SmokeScreen,
           MoveCharacter.Instance.rigidBody.position, Quaternion.identity);
        NetworkServer.Spawn(gameObject);
    }

    void Awake(){
        if(gameObject != null){
            Destroy(gameObject, 10.0f);
        }
    }
}