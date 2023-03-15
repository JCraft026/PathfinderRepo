using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class SpawnGenerators : NetworkBehaviour
{
    public static int spawnCount = 3;
    public GameObject steamGenerator;
    public static int generatedSteam = 0; // Steam currently available to the guardmaster. 

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

    public static void generateGeneratorLocations()
    {
        Debug.Log("Generating generators");
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
            GameObject.Find("ItemAssets").GetComponent<CommandManager>().NetworkedSpawnGenerator(generatorPos);

            // Make sure we can't spawn 2 generators at the same location
            topWalls.Remove(topWalls[wallIndex]);
        }
    }
}
