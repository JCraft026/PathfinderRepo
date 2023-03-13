using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class RenderSmokeScreen : NetworkBehaviour
{
    public static RenderSmokeScreen Instance; // Makes an instance of this class to access attribtuess
    private static int smokeScreensSpawned = 0; // Number of smoke screens that have been spawned throughout the game (obviously 0 based)
    public int smokeScreenNum;
    MoveCharacter runnerScript {get {return Resources.FindObjectsOfTypeAll<GameObject>()
                                            .FirstOrDefault(gObject => gObject
                                                .name.Contains("Runner"))
                                            .GetComponent<MoveCharacter>();}}
    // Called on Start
    void Start(){
        if(Instance == null)
        {
            Instance = this;
        }
        smokeScreenNum = smokeScreensSpawned;
        smokeScreensSpawned += 1;
        //runnerScript = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).GetComponent<MoveCharacter>();    
        }
    // Update is called once per frame
    [Command(requiresAuthority = false)]
    public void useSmoke(){
        GameObject gObject = Instantiate(ItemAssets.Instance.SmokeScreen,
           runnerScript.rigidBody.position, Quaternion.identity);
           gObject.name = gObject.name + smokeScreensSpawned.ToString();
           
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

        // Destroy the earliest smoke screen
        List<GameObject> smokeScreens = GameObject.FindObjectsOfType<GameObject>().Where<GameObject>(x => x.name.Contains("Smoke") && x.GetComponent<RenderSmokeScreen>().smokeScreenNum != 0).ToList();

        //Cringey bubble sort to figure out which smoke screen is the oldest
        for(int behind = 0; behind < smokeScreens.Count - 1; behind++)
        {
            for(int forward = behind; forward < smokeScreens.Count; forward++)
            {
                if(smokeScreens[behind].GetComponent<RenderSmokeScreen>().smokeScreenNum > smokeScreens[forward].GetComponent<RenderSmokeScreen>().smokeScreenNum)
                {
                    GameObject tempSmoke = smokeScreens[behind];
                    smokeScreens[behind] = smokeScreens[forward];
                    smokeScreens[forward] = tempSmoke;
                }
            }
        }
        NetworkServer.Destroy(smokeScreens[0]);
    }
}