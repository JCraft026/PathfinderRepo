using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SteamGenerators : MonoBehaviour
{
    public GameObject steamGenerator;
    public Transform steamGeneratorP;
    public int spawnCount = 3;


    private void Start()
    {
        spawnGenerators();
    }

    void spawnGenerators()
    {        
        for (int spawnLimit = 1; spawnLimit <= spawnCount; spawnLimit++)
        {
            Vector3 steamGeneratorPos = new Vector3(Random.Range(10, -10), Random.Range(10, -10), 0);
            Instantiate(steamGenerator, steamGeneratorPos, Quaternion.identity);
        }
    }

    /*
    void Start()
    {
        List<GameObject> Walls = Resources.FindObjectsOfTypeAll<GameObject>().Select(x => 
        {
            if (x.name.Contains("Wall") || x.name.Contains("Exit"))
                return x;
            else
                return null;
        }).ToList();

        Walls.ForEach(x => {
            if (x.gameObject.transform.position == transform.position)
                transform.position = new Vector3(Random.value, Random.value, Random.value);
                });

        Instantiate(steamGenerator, transform.position, Quaternion.identity);*/
}
