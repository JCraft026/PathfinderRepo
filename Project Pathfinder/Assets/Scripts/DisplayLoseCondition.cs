using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayLoseCondition : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Display the Lose status messages for both the guard master and the runner
        switch (HandleEvents.endGameEvent)
        {
            // Display the Lose status messages for the runner
            case HandleEventsConstants.RUNNER_CAPTURED:
                GameObject.Find("Lose Condition").GetComponent<TextMeshPro>().text = "You were captured by the guard master";
                break;
            case HandleEventsConstants.TIMER_ZERO:
                GameObject.Find("Lose Condition").GetComponent<TextMeshPro>().text = "You ran out of time";
                break;
            case HandleEventsConstants.RUNNER_TRAPPED:
                GameObject.Find("Lose Condition").GetComponent<TextMeshPro>().text = "You were defeated by a trap chest";
                break;
                
            // Display the Lose status messages for the guard master
            case HandleEventsConstants.RUNNER_ESCAPED:
                GameObject.Find("Lose Condition").GetComponent<TextMeshPro>().text = "The Runner escaped the maze";
                break;
        }
    }
}