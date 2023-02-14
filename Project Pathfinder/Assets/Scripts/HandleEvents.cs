using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

static class HandleEventsConstants{
    public const int NONE        = 0; // No event active
    public const int RUNNER_WINS = 1; // Runner win event
}

public class HandleEvents : MonoBehaviour
{
    public static int currentEvent = 0;            // Current game event to be handled
    public static Vector3 activeCharacterLocation; // Scene position of the current active character

    // Update is called once per frame
    void Update()
    {
        // Handle the current game event
        switch (currentEvent)
        {
            case HandleEventsConstants.RUNNER_WINS:
                SceneManager.LoadScene("Player Wins");
                currentEvent = HandleEventsConstants.NONE;
                break;
        }

        // Update the location of the active character
        UpdateActiveCharacterLocation();
    }

    public void UpdateActiveCharacterLocation(){
        int   activeGuardID;       // Guard code of the current active guard

        if(CustomNetworkManager.isRunner){
            activeCharacterLocation = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).transform.position;
        }
        else{
            activeGuardID = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<ManageActiveCharacters>().activeGuardId;
            switch (activeGuardID)
            {
                case ManageActiveCharactersConstants.CHASER:
                    activeCharacterLocation = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).transform.position;
                    break;
                case ManageActiveCharactersConstants.ENGINEER:
                    activeCharacterLocation = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer")).transform.position;
                    break;
                case ManageActiveCharactersConstants.TRAPPER:
                    activeCharacterLocation = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper")).transform.position;
                    break;
            }
        }
    }
}
