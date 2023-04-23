using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ManageFastTravel : MonoBehaviour
{
    bool chaserTimerActive   = false; // Status of running chaser transport timer
    bool engineerTimerActive = false; // Status of running engineer transport timer
    bool trapperTimerActive  = false; // Status of running trapper transport timer
    float chaserTimeLeft     = 5.5f;    // Seconds remaining on chaser timer
    float engineerTimeLeft   = 5.5f;    // Seconds remaining on engineer timer
    float trapperTimeLeft    = 5.5f;    // Seconds remaing on trapper timer

    // Update is called once per frame
    void Update()
    {
        // Manage active chaser transport timer
        if(chaserTimerActive){
            if(chaserTimeLeft > 0){
                GameObject.Find("Chaser Transport Timer").GetComponent<TextMeshPro>().text = ((int)chaserTimeLeft).ToString();
                chaserTimeLeft -= Time.deltaTime;
            }
            else{
                chaserTimeLeft    = 5.5f;
                chaserTimerActive = false;
                GameObject.Find("Chaser Transport Timer").GetComponent<TextMeshPro>().text = ((int)chaserTimeLeft).ToString();
            }
        }

        // Manage active engineer transport timer
        if(engineerTimerActive){
            if(engineerTimeLeft > 0){
                GameObject.Find("Engineer Transport Timer").GetComponent<TextMeshPro>().text = ((int)engineerTimeLeft).ToString();
                engineerTimeLeft -= Time.deltaTime;
            }
            else{
                engineerTimeLeft    = 5.5f;
                engineerTimerActive = false;
                GameObject.Find("Engineer Transport Timer").GetComponent<TextMeshPro>().text = ((int)engineerTimeLeft).ToString();
            }
        }

        // Manage active trapper transport timer
        if(trapperTimerActive){
            if(trapperTimeLeft > 0){
                GameObject.Find("Trapper Transport Timer").GetComponent<TextMeshPro>().text = ((int)trapperTimeLeft).ToString();
                trapperTimeLeft -= Time.deltaTime;
            }
            else{
                trapperTimeLeft    = 5.5f;
                trapperTimerActive = false;
                GameObject.Find("Trapper Transport Timer").GetComponent<TextMeshPro>().text = ((int)trapperTimeLeft).ToString();
            }
        }
    }

    // Fast travel a guard to its destination cell
    public static void FastTravel(int guardId, Vector3 destination){
        GameObject guard; // Guard game object cooresponding to guardId

        if(guardId == ManageActiveCharactersConstants.CHASER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        }
        else{
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        }

        guard.GetComponent<Animator>().SetFloat("Fast Travel X", destination.x);
        guard.GetComponent<Animator>().SetFloat("Fast Travel Y", destination.y);
        guard.GetComponent<Animator>().SetBool("Fast Travel Started", true);
        guard.GetComponent<MoveCharacter>().facingDirection = MoveCharacterConstants.FORWARD;
        guard.GetComponent<MoveCharacter>().flashlight.transform.eulerAngles = new Vector3(0f, 0f, 180f);
    }

    // Start coroutine to wait through fast travel idle time
    public void InitiateFastTravelIdle(int guardId){
        StartCoroutine(WaitThroughIdle(guardId));
    }

    // Display and wait through fast travel idle time
    IEnumerator WaitThroughIdle(int guardId)
    {
        GameObject guard; // Guard game object cooresponding to guardId

        if(guardId == ManageActiveCharactersConstants.CHASER){
            chaserTimerActive = true;
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            engineerTimerActive = true;
        }
        else{
            trapperTimerActive = true;
        }

        yield return new WaitForSeconds(5);

        if(guardId == ManageActiveCharactersConstants.CHASER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
        }
        else if(guardId == ManageActiveCharactersConstants.ENGINEER){
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
        }
        else{
            guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
        }

        guard.GetComponent<Animator>().SetBool("Fast Travel Finished", true);
    }
}
