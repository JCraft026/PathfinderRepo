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
    public string firstExitName  = "Exit1";
    public string secondExitName = "Exit2";
    public string thirdExitName  = "Exit3";
    public string fourthExitName = "Exit4";
    public string firstKeyholeName  = "Keyhole1";
    public string secondKeyholeName = "Keyhole2";
    public string thirdKeyholeName  = "Keyhole3";
    public string fourthKeyholeName = "Keyhole4";
    public int lockID;                                      // ID of the current lock
    public bool lockUnlocked         = false;               // Status of lock being unlocked

    // Start is called before the first frame update
    void Start()
    {
        AssignLockID();
    }

    // Update is called once per frame
    void Update()
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        if(CustomNetworkManager.isRunner && runner.GetComponent<Animator>().GetBool("Unlock Attempted")){
            if(gameObject.transform.position.y - runner.transform.position.y > 0.0f && gameObject.transform.position.y - runner.transform.position.y < 3.0f && runner.transform.position.x - gameObject.transform.position.x > -1.0f && runner.transform.position.x - gameObject.transform.position.x < 1.0f && lockUnlocked == false){
                switch (lockID)
                {
                    case ManageUnlockAttemptConstants.GREEN: 
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Open)")).SetActive(true);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstExitName+"(Close)")).SetActive(false);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Unlocked)")).SetActive(true);
                        StartCoroutine(GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().DisplayPopup("First Exit Opened", 5));
                        lockUnlocked = true;
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(firstKeyholeName+"(Locked)")).SetActive(false);
                        break;
                    case ManageUnlockAttemptConstants.YELLOW: 
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Open)")).SetActive(true);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondExitName+"(Close)")).SetActive(false);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Unlocked)")).SetActive(true);
                        StartCoroutine(GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().DisplayPopup("Second Exit Opened", 5));
                        lockUnlocked = true;
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(secondKeyholeName+"(Locked)")).SetActive(false);
                        break;
                    case ManageUnlockAttemptConstants.BLUE:
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Open)")).SetActive(true);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdExitName+"(Close)")).SetActive(false);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Unlocked)")).SetActive(true);
                        StartCoroutine(GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().DisplayPopup("Third Exit Opened", 5));
                        lockUnlocked = true;
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(thirdKeyholeName+"(Locked)")).SetActive(false);
                        break;
                    case ManageUnlockAttemptConstants.RED: 
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Open)")).SetActive(true);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthExitName+"(Close)")).SetActive(false);
                        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains(fourthKeyholeName+"(Unlocked)")).SetActive(true);
                        StartCoroutine(GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().DisplayPopup("Fourth Exit Opened", 5));
                        lockUnlocked = true;
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
