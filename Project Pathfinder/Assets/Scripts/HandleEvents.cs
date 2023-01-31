using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

static class HandleEventsConstants{
    public const int NONE        = 0; // No event active
    public const int RUNNER_WINS = 1; // Runner win event
}

public class HandleEvents : MonoBehaviour
{
    public static int currentEvent = 0; // Current game event to be handled

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
    }

    // Manipulate runner animations and hp to reflect the landed guard attack
    public static void ProcessAttackImpact(int direction){
        switch ((float)direction)
        {
            case MoveCharacterConstants.FORWARD:
                Debug.Log("Guard struck runner from above");
                break;
            case MoveCharacterConstants.LEFT:
                Debug.Log("Guard struck runner from the right");
                break;
            case MoveCharacterConstants.BACKWARD:
                Debug.Log("Guard struck runner from below");
                break;
            case MoveCharacterConstants.RIGHT:
                Debug.Log("Guard struck runner from the left");//
                break;
        }
    }
}
