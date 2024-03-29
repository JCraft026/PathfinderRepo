using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

static class ManageUnlockAttemptConstants{
    // Lock IDs
    public const int GREEN  = 1;
    public const int YELLOW = 2;
    public const int BLUE   = 3;
    public const int RED    = 4;
}

public class ManageUnlockAttempt : MonoBehaviour
{
    public string firstExitName      = "Exit1";
    public string secondExitName     = "Exit2";
    public string thirdExitName      = "Exit3";
    public string fourthExitName     = "Exit4";
    public string firstKeyholeName   = "Keyhole1";
    public string secondKeyholeName  = "Keyhole2";
    public string thirdKeyholeName   = "Keyhole3";
    public string fourthKeyholeName  = "Keyhole4";
    public int lockID;                             // ID of the current lock
    public bool lockUnlocked         = false;      // Status of lock being unlocked
    public bool greenExitOpened      = false;      // Status of green exit being opened on guard master side
    public bool yellowExitOpened     = false;      // Status of yellow exit being opened on guard master side
    public bool blueExitOpened       = false;      // Status of blue exit being opened on guard master side
    public bool redExitOpened        = false;      // Status of red exit being opened on guard master side
    private Player_UI playerUi;                    // Player UI management script

    // Start is called before the first frame update
    void Start()
    {
        // Assign Player UI script
        playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();

        // Assign lock IDs
        AssignLockID();
        
    }

    // Update is called once per frame
    void Update()
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
            // Runner game object

        // Show unlock instruction above lock if the runner is close
        if(CustomNetworkManager.IsRunner){
            if(gameObject.transform.position.y - runner.transform.position.y > 0.0f && gameObject.transform.position.y - runner.transform.position.y < 1.5f && runner.transform.position.x - gameObject.transform.position.x > -0.75f && runner.transform.position.x - gameObject.transform.position.x < 0.75f && lockUnlocked == false){
                switch (lockID)
                {
                    case ManageUnlockAttemptConstants.GREEN: 
                        if(playerUi.hasKey2){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction1")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.YELLOW: 
                        if(playerUi.hasKey3){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction2")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.BLUE:
                        if(playerUi.hasKey1){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction3")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.RED: 
                        if(playerUi.hasKey0){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction4")).SetActive(true);
                        }
                        break;
                }
            }
            else{
                switch (lockID)
                {
                    case ManageUnlockAttemptConstants.GREEN: 
                        if(playerUi.hasKey2){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction1")).SetActive(false);
                        }
                        break;
                    case ManageUnlockAttemptConstants.YELLOW: 
                        if(playerUi.hasKey3){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction2")).SetActive(false);
                        }
                        break;
                    case ManageUnlockAttemptConstants.BLUE:
                        if(playerUi.hasKey1){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction3")).SetActive(false);
                        }
                        break;
                    case ManageUnlockAttemptConstants.RED: 
                        if(playerUi.hasKey0){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("UnlockInstruction4")).SetActive(false);
                        }
                        break;
                }
            }
        }

        // Process opening exits on the runner side
        if(CustomNetworkManager.IsRunner && runner.GetComponent<Animator>().GetBool("Unlock Attempted")){
            
            // If the runner is standing in front of the parent object keyhole, open the cooresponding exit and display the appropriate popup message
            if(gameObject.transform.position.y - runner.transform.position.y > 0.0f && gameObject.transform.position.y - runner.transform.position.y < 1.5f && runner.transform.position.x - gameObject.transform.position.x > -0.75f && runner.transform.position.x - gameObject.transform.position.x < 0.75f && lockUnlocked == false){
                switch (lockID)
                {
                    case ManageUnlockAttemptConstants.GREEN: 
                        if(playerUi.hasKey2){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Open)")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Close)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Unlocked)")).SetActive(true);
                            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=green>Green</color> Exit Opened!", 5f);
                            lockUnlocked = true;
                            runner.GetComponent<Animator>().SetBool("Green Exit Opened", true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Locked)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit1Open")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.YELLOW: 
                        if(playerUi.hasKey3){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Open)")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Close)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Unlocked)")).SetActive(true);
                            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=yellow>Yellow</color> Exit Opened!", 5f);
                            lockUnlocked = true;
                            runner.GetComponent<Animator>().SetBool("Yellow Exit Opened", true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Locked)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit2Open")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.BLUE:
                        if(playerUi.hasKey1){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Open)")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Close)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Unlocked)")).SetActive(true);
                            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=#2f2ff5>Blue</color> Exit Opened!", 5f);
                            lockUnlocked = true;
                            runner.GetComponent<Animator>().SetBool("Blue Exit Opened", true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Locked)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit3Open")).SetActive(true);
                        }
                        break;
                    case ManageUnlockAttemptConstants.RED: 
                        if(playerUi.hasKey0){
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Open)")).SetActive(true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Close)")).SetActive(false);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthKeyholeName+"(Unlocked)")).SetActive(true);
                            GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=red>Red</color> Exit Opened!", 5f);
                            lockUnlocked = true;
                            runner.GetComponent<Animator>().SetBool("Red Exit Opened", true);
                            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit4Open")).SetActive(true);
                        }
                        break;
                }
            }
            if(lockID == ManageUnlockAttemptConstants.RED){
                runner.GetComponent<Animator>().SetBool("Unlock Attempted", false);
                if(lockUnlocked == true){
                    Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthKeyholeName+"(Locked)")).SetActive(false);
                }
            }
        }

        // Process opening exits on guard master side
        else if(GameObject.Find(runner.name) != null){
            if(runner.GetComponent<Animator>().GetBool("Green Exit Opened") && greenExitOpened == false){
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Open)")).SetActive(true);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Close)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Unlocked)")).SetActive(true);
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=green>Green</color> Exit Opened!", 5f);
                greenExitOpened = true;
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Locked)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit1Open")).SetActive(true);
            }
            if(runner.GetComponent<Animator>().GetBool("Yellow Exit Opened") && yellowExitOpened == false){
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Open)")).SetActive(true);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Close)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Unlocked)")).SetActive(true);
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=yellow>Yellow</color> Exit Opened!", 5f);
                yellowExitOpened = true;
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Locked)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit2Open")).SetActive(true);
            }
            if(runner.GetComponent<Animator>().GetBool("Blue Exit Opened") && blueExitOpened == false){
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Open)")).SetActive(true);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Close)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Unlocked)")).SetActive(true);
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=#2f2ff5>Blue</color> Exit Opened!", 5f);
                blueExitOpened = true;
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Locked)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit3Open")).SetActive(true);
            }
            if(runner.GetComponent<Animator>().GetBool("Red Exit Opened") && redExitOpened == false){
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Open)")).SetActive(true);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Close)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthKeyholeName+"(Unlocked)")).SetActive(true);
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=red>Red</color> Exit Opened!", 5f);
                redExitOpened = true;
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthKeyholeName+"(Locked)")).SetActive(false);
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("MMExit4Open")).SetActive(true);
            }
        }
    }

    // Assign the appropriate lock ID to the current lock
    public void AssignLockID(){
        if(gameObject.name == firstKeyholeName+"(Locked)"){
            lockID = ManageUnlockAttemptConstants.GREEN;
        }
        else if(gameObject.name == secondKeyholeName+"(Locked)"){
            lockID = ManageUnlockAttemptConstants.YELLOW;
        }
        else if(gameObject.name == thirdKeyholeName+"(Locked)"){
            lockID = ManageUnlockAttemptConstants.BLUE;
        }
        else if(gameObject.name == fourthKeyholeName+"(Locked)"){
            lockID = ManageUnlockAttemptConstants.RED;
        }
    }
}
