using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

static class HandleEventsConstants{
    public const int NONE             = 0; // No event active
    public const int RUNNER_WINS      = 1; // Runner win event
    public const int GUARDMASTER_WINS = 2; // Guard Master win event
    public const int RUNNER_CAPTURED  = 3; // Runner was captured
    public const int RUNNER_ESCAPED   = 4; // Runner escaped the maze
    public const int TIMER_ZERO       = 5; // Timer has run down to zero
    public const int RUNNER_TRAPPED   = 6; // Runner has been defeated by a trap chest
}

public class HandleEvents : MonoBehaviour
{
    public static int currentEvent = 0;            // Current game event to be handled
    public static int endGameEvent = 0;            // Reason the game was ended

    // Update is called once per frame
    void Update()
    {
        // Handle the current game event
        switch (currentEvent)
        {
            case HandleEventsConstants.RUNNER_WINS:
                if(CustomNetworkManager.IsRunner){
                    SceneManager.LoadScene("Player Wins");
                }
                else{
                    SceneManager.LoadScene("Player Loses");
                }
                Resources.FindObjectsOfTypeAll<GameObject>()
                        .Where<GameObject>( x => x.GetComponent<CommandManager>() != null)
                        .First()
                        .GetComponent<CommandManager>()
                        .cmd_TransitionToYouWinYouLose(currentEvent, endGameEvent);
                currentEvent = HandleEventsConstants.NONE;
                break;
            case HandleEventsConstants.GUARDMASTER_WINS:
                if(!CustomNetworkManager.IsRunner){
                    SceneManager.LoadScene("Player Wins");
                }
                else{
                    SceneManager.LoadScene("Player Loses");
                }
                Resources.FindObjectsOfTypeAll<GameObject>()
                        .Where<GameObject>( x => x.GetComponent<CommandManager>() != null)
                        .First()
                        .GetComponent<CommandManager>()
                        .cmd_TransitionToYouWinYouLose(currentEvent, endGameEvent);
                currentEvent = HandleEventsConstants.NONE;
                break;
        }

        // Disable start screen for client
        if(!CustomNetworkManager.IsHost){
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("OpponentJoined")).SetActive(false);
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("WaitingForOpponent")).SetActive(false);
        }
    }

    // Manipulate runner animations and hp to reflect the landed guard attack
    public static void ProcessAttackImpact(int direction){
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            // Runner character object

        // Set up animator for displaying guard attack impact
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
