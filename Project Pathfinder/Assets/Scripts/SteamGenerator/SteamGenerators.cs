using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class SteamGenerators : NetworkBehaviour
{
    public static int spawnCount = 3;
    public GameObject steamGenerator;
    public bool isBroken = false;
    public static int generatedSteam = 0; // Steam currently available to the guardmaster. 
    public Animator animator;
    public Animation newAnimation;

    private void Start()
    {
        StartCoroutine(GenerateSteam());
    }

    public void SpawnGenerators()
    {
        List<GameObject> topWalls = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where<GameObject>(x => x.name.Contains("Wall_TB")).ToList();

        for (int spawnLimit = 1; spawnLimit <= spawnCount; spawnLimit++)
        {
            int wallIndex = UnityEngine.Random.Range(0, topWalls.Count);
            Vector2 generatorPos;
            generatorPos = new Vector2(topWalls[wallIndex].transform.position.x, topWalls[wallIndex].transform.position.y - 5);

            if(generatorPos.y >= 50 || generatorPos.y <= -50)
            {
                if (generatorPos.y >= 50)
                    generatorPos.y -= generatorPos.y - 56;
                else
                    generatorPos.y += generatorPos.y - 56;
            }

                var gObject = Instantiate(steamGenerator, generatorPos, Quaternion.identity);
                NetworkedSpawnGenerator(gObject);
                topWalls.Remove(topWalls[wallIndex]);
        }
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
            }
            else if(generatedSteam > 100)
            {
                generatedSteam = 100;
                //animator.animation = newAnimation;
            }
        }
    }

    public bool GeneratorBreak()
    {
        if(isBroken == false)
        {
            isBroken = true;
            // Implement sprite change
        }
        return isBroken;
    }

    [Command(requiresAuthority = false)]
    public void NetworkedSpawnGenerator(GameObject generator)
    {
        NetworkServer.Spawn(generator);
    }
}
