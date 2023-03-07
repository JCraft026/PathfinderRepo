using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class RenderSmokeScreen : NetworkBehaviour
{
    public static RenderSmokeScreen Instance; // Makes an instance of this class to access attribtuess
    MoveCharacter runnerScript {get {return Resources.FindObjectsOfTypeAll<GameObject>()
                                            .FirstOrDefault(gObject => gObject
                                                .name.Contains("Runner"))
                                            .GetComponent<MoveCharacter>();}}
    // Called on Start
    void Start(){
        Instance = this;

        //runnerScript = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).GetComponent<MoveCharacter>();    
        }
    // Update is called once per frame
    [Command(requiresAuthority = false)]
    public void useSmoke(){
        GameObject gObject = Instantiate(ItemAssets.Instance.SmokeScreen,
           runnerScript.rigidBody.position, Quaternion.identity);
        NetworkServer.Spawn(gObject);
        StartSmokeFade(this.gameObject);
    }

    void Awake(){
    }

    // Tells the server to start the timer
    [Command(requiresAuthority = false)]
    public void StartSmokeFade(GameObject smoke)
    {
        StartCoroutine(SmokeFade(smoke));
    }

    // Destroy the smoke cloud
    IEnumerator SmokeFade(GameObject smoke){
        long initialTime = DateTime.Now.Ticks;

        // Wait 10 seconds
        long tenSeconds = 100000000;
        long result = DateTime.Now.Ticks - initialTime;
        while(result < tenSeconds)
        {
            result = DateTime.Now.Ticks - initialTime;
            Debug.Log(result);
            yield return null;
        }
        Debug.Log(smoke.name + " should be destroyed");
        //NetworkServer.Destroy(smoke);
        NetworkServer.Destroy(Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Smoke")));
    }

    // Destroys the smoke on the client side (We shouldn't have to use this but I have it just in case)
    [ClientRpc]
    public void DestroySmoke(GameObject smoke)
    {
        NetworkServer.Destroy(Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Smoke")));
    }
}