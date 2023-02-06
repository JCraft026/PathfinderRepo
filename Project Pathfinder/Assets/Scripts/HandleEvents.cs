using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

static class HandleEventsConstants{
    public const int NONE        = 0; // No event active
    public const int RUNNER_WINS = 1; // Runner win event
}

public class HandleEvents : MonoBehaviour
{
    public static int currentEvent = 0;     // Current game event to be handled

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
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));

        switch ((float)direction)
        {
            case MoveCharacterConstants.FORWARD:
                runner.GetComponent<Animator>().SetFloat("Impact Direction", MoveCharacterConstants.BACKWARD);
                runner.GetComponent<Animator>().SetBool("Hurt", true);
                break;
            case MoveCharacterConstants.LEFT:
                runner.GetComponent<Animator>().SetFloat("Impact Direction", MoveCharacterConstants.LEFT);
                runner.GetComponent<Animator>().SetBool("Hurt", true);
                break;
            case MoveCharacterConstants.BACKWARD:
                runner.GetComponent<Animator>().SetFloat("Impact Direction", MoveCharacterConstants.FORWARD);
                runner.GetComponent<Animator>().SetBool("Hurt", true);
                break;
            case MoveCharacterConstants.RIGHT:
                runner.GetComponent<Animator>().SetFloat("Impact Direction", MoveCharacterConstants.RIGHT);
                runner.GetComponent<Animator>().SetBool("Hurt", true);
                break;
        }
    }
}
