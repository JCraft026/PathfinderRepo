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

        // Seeds the random number for each game instance
        Random.state.Equals((int)System.DateTime.Now.Ticks);

        // Declare a list of possible spawning locations
        List<int> listOfSpawnSpots = new List<int>() {0,1,2,3};

        // Randomly select, use, and remove one of the options of where to spawn a generator 
        for (int spawnLimit = 1; spawnLimit <= spawnCount;)
        {
            Vector2 generatorPos = new Vector2();
            int spawnPlace = UnityEngine.Random.Range(0, 4);

            if(listOfSpawnSpots.Contains(spawnPlace)){
                switch(spawnPlace){
                    case 0: 
                        listOfSpawnSpots.Remove(spawnPlace);
                        spawnLimit += 1;
                        generatorPos = new Vector2(24,25);
                        Debug.Log("Generating generators: case 0");
                        break;
                    case 1:
                        listOfSpawnSpots.Remove(spawnPlace);
                        spawnLimit += 1;
                        generatorPos = new Vector2(24,-23);
                        Debug.Log("Generating generators: case 1");
                        break;
                    case 2: 
                        listOfSpawnSpots.Remove(spawnPlace);
                        spawnLimit += 1;
                        generatorPos = new Vector2(-24,25);
                        Debug.Log("Generating generators: case 2");
                        break;
                    case 3:
                        listOfSpawnSpots.Remove(spawnPlace); 
                        spawnLimit += 1;
                        generatorPos = new Vector2(-24,-23);
                        Debug.Log("Generating generators: case 3");
                        break;
                    default: 
                        Debug.Log("Not one of the 4 steam generator spawn spots picked");
                        break;
                }

                // Spawn the steam generator
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().NetworkedSpawnGenerator(generatorPos);
            }
            else{
                Debug.Log("The number " + spawnPlace + " is not in the range");
            }
        }
    }
}
