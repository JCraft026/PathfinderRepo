using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageFastTravel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Fast travel a guard to its destination cell
    public static void FastTravel(int guardId, Vector3 destination){
        GameObject guard; // Guard game object cooresponding to guardId

        if(guardId == ManageActiveCharactersConstants.CHASER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
        }
        else{
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));
        }

        guard.transform.position = destination;
    }
}
