using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayWinCondition : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Display the Win status messages for both the guard master and the runner
        switch (HandleEvents.endGameEvent)
        {
            // Display the Win status messages for the guard master
            case HandleEventsConstants.RUNNER_CAPTURED:
                GameObject.Find("Win Condition").GetComponent<TextMeshPro>().text = "You captured the runner!";
                break;
            case HandleEventsConstants.TIMER_ZERO:
                GameObject.Find("Win Condition").GetComponent<TextMeshPro>().text = "The runner ran out of time!";
                break;
                
            // Display the Win status messages for the runner
            case HandleEventsConstants.RUNNER_ESCAPED:
                GameObject.Find("Win Condition").GetComponent<TextMeshPro>().text = "You escaped the maze!";
                break;
        }
    }
}
