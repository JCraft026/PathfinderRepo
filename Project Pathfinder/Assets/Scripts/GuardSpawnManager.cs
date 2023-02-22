using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpawnManager : MonoBehaviour
{
    public static bool ranOnceAlready = false;
    // Start is called before the first frame update
    void Start()
    {
        if(ranOnceAlready == false)
            CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().SetGuardSpawnLocations();
    }
}
