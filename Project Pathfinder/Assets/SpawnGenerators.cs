using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class SteamGenerators : NetworkBehaviour
{
    public static int spawnCount = 3;
    public GameObject steamGenerator;
    public bool isBroken = false;
    public static int generatedSteam = 0; // Steam currently available to the guardmaster. 
    public Animator animator;
    private Animator generatorAnimator;

    void Start()
    {
        // Remove steam bar if the player is the runner
        if (CustomNetworkManager.isRunner == true)
        {
            try{
                GameObject.FindGameObjectWithTag("SteamBar").SetActive(false);
            }
            catch{

            }
        }
    }

    public static void SpawnGenerators()
    {
        // Get a list of walls to spawn near
        List<GameObject> topWalls = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where<GameObject>(x => x.name.Contains("Wall_TB")).ToList();

        for (int spawnLimit = 1; spawnLimit <= spawnCount; spawnLimit++)
        {
            // Pick a random wall to spawn the generator at
            int wallIndex = UnityEngine.Random.Range(0, topWalls.Count);
            Vector2 generatorPos;
            generatorPos = new Vector2(topWalls[wallIndex].transform.position.x, topWalls[wallIndex].transform.position.y - 5);

            // Adjust the generator coordinates to fit within maze bounds (sorry its all hardcoded! -Caleb)
            if(generatorPos.y >= 50 || generatorPos.y <= -50)
            {
                if (generatorPos.y >= 50)
                    generatorPos.y -= generatorPos.y - 56;
                else
                    generatorPos.y += generatorPos.y - 56;
            }

            // Get the steam generator prefab from the network manager
            var generatorPrefab = CustomNetworkManagerDAO.GetNetworkManagerGameObject()
                                    .GetComponent<CustomNetworkManager>().spawnPrefabs
                                    .Find(x => x.name.Contains("Steam Generator"));

            // Spawn the steam generator
            // FIX //GameObject.Find("ItemAssets").GetComponent<CommandManager>().NetworkedSpawnGenerator(generatorPos);

            // Make sure we can't spawn 2 generators at the same location
            topWalls.Remove(topWalls[wallIndex]);
        }
    }

    public void StartGeneratingSteam()
    {
        if(CustomNetworkManager.isRunner == false)
            StartCoroutine(GenerateSteam());
    }

    // Asyncronously generates 1 steam point every second (as long as the generator is not broken)
    IEnumerator GenerateSteam()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            if(isBroken == false && generatedSteam < 100)
            {
                generatedSteam += 1;
                animator.enabled = true;
            }
            else if(generatedSteam > 100)
            {
                generatedSteam = 100;
                animator.enabled = false;
            }

	        if (generatedSteam == 0)
            {
                GameObject.Find("SteamBar0").SetActive(true);
                GameObject.Find("SteamBar25").SetActive(false);
            }
            else if (generatedSteam == 25)
            {
                GameObject.Find("SteamBar0").SetActive(false);
                GameObject.Find("SteamBar25").SetActive(true);
                GameObject.Find("SteamBar50").SetActive(false);
            }
            else if (generatedSteam == 50)
            {
                GameObject.Find("SteamBar25").SetActive(false);
                GameObject.Find("SteamBar50").SetActive(true);
                GameObject.Find("SteamBar75").SetActive(false);
            }
            else if (generatedSteam == 75)
            {
                GameObject.Find("SteamBar50").SetActive(false);
                GameObject.Find("SteamBar75").SetActive(true);
                GameObject.Find("SteamBar100").SetActive(false);
            }
            else if (generatedSteam == 100)
            {
                GameObject.Find("SteamBar75").SetActive(false);
                GameObject.Find("SteamBar100").SetActive(true);
                // Turn off animation
            }
        }
    }

    public bool GeneratorBreak()
    {
        if(isBroken == false)
        {
            isBroken = true;
            generatorAnimator.SetBool("isBusted", true);
        }
        return isBroken;
    }

    // Anne's new version
    public bool GeneratorFixed()
    {
        if (isBroken == true)
        {
            isBroken = false;
            generatorAnimator.SetBool("isBusted", false);
        }
        return isBroken;
    }
}
