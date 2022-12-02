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
}
