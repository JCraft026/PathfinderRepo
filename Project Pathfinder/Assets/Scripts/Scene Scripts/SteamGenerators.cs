using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class SteamGenerators : NetworkBehaviour
{
    public int spawnCount = 3;
    public GameObject steamGenerator;

    private void Start()
    {
        spawnGenerators();
    }

    void spawnGenerators()
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
    [Command(requiresAuthority = false)]
    public void NetworkedSpawnGenerator(GameObject generator)
    {
        NetworkServer.Spawn(generator);
    }
}
