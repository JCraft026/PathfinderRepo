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

        guard.GetComponent<Animator>().SetFloat("Fast Travel X", destination.x);
        guard.GetComponent<Animator>().SetFloat("Fast Travel Y", destination.y);
        guard.GetComponent<Animator>().SetBool("Fast Travel Started", true);
    }

    // Start coroutine to wait through fast travel idle time
    public void InitiateFastTravelIdle(int guardId){
        StartCoroutine(WaitThroughIdle(guardId));
    }

    // Display and wait through fast travel idle time
    IEnumerator WaitThroughIdle(int guardId)
    {
        GameObject guard; // Guard game object cooresponding to guardId

        yield return new WaitForSeconds(5);

        if(guardId == ManageActiveCharactersConstants.CHASER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
        }
        else{
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));
        }

        guard.GetComponent<Animator>().SetBool("Fast Travel Finished", true);
    }
}
