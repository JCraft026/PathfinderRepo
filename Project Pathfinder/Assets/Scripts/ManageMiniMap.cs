using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageMiniMap : MonoBehaviour
{
    // Initialize fields in the inspector
    [SerializeField]
    private Transform runnerIconPrefab = null;
    [SerializeField]
    private Transform chaserIconPrefab = null;
    [SerializeField]
    private Transform engineerIconPrefab = null;
    [SerializeField]
    private Transform trapperIconPrefab = null;

    private int[] runnerCellLocation      = new int[]{0,0}; // Old runner cell location
    private int[] chaserCellLocation      = new int[]{0,0}; // Old chaser cell location
    private int[] engineerCellLocation    = new int[]{0,0}; // Old engineer cell location
    private int[] trapperCellLocation     = new int[]{0,0}; // Old trapper cell location
    private int[] newRunnerCellLocation   = new int[]{0,0}; // New runner cell location
    private int[] newChaserCellLocation   = new int[]{0,0}; // New chaser cell location
    private int[] newEngineerCellLocation = new int[]{0,0}; // New engineer cell location
    private int[] newTrapperCellLocation  = new int[]{0,0}; // New trapper cell location
    
    private Transform runnerIcon;   // Runner icon game object
    private Transform chaserIcon;   // Chaser icon game object
    private Transform engineerIcon; // Engineer icon game object
    private Transform trapperIcon;  // Trapper icon game object

    public bool componentsInitialized = false; // Reflects character icon initialization
    public float nextSpawnTime;                // Next time to flash the runner icon
    public float waitTime = 0.25f;             // Next time to flash the runner icon

    // Start is called before the first frame update
    public void InitializeMinimapComponents(){
        float cellSize = Utilities.GetMapCellSize(); // Minimap cell size

        // If the user is playing as the guard master, initialize guard mini map icons
        if(CustomNetworkManager.IsRunner == false){
            // Initialize chaser minimap icon
            chaserIcon = Instantiate(chaserIconPrefab, transform);
            chaserIcon.transform.SetParent(GameObject.Find("Minimap").transform, false);
            chaserIcon.GetComponent<RectTransform>().localScale = new Vector2((cellSize/2), (cellSize/2));
            chaserIcon.name = "ChaIcon";

            // Initialize engineer minimap icon
            engineerIcon = Instantiate(engineerIconPrefab, transform);
            engineerIcon.transform.SetParent(GameObject.Find("Minimap").transform, false);
            engineerIcon.GetComponent<RectTransform>().localScale = new Vector2((cellSize/2), (cellSize/2));
            engineerIcon.name = "EngIcon";

            // Initialize trapper minimap icon
            trapperIcon = Instantiate(trapperIconPrefab, transform);
            trapperIcon.transform.SetParent(GameObject.Find("Minimap").transform, false);
            trapperIcon.GetComponent<RectTransform>().localScale = new Vector2((cellSize/2), (cellSize/2));
            trapperIcon.name = "TraIcon";

            // Initialize the runner minimap icon
            runnerIcon = Instantiate(runnerIconPrefab, transform);
            runnerIcon.transform.SetParent(GameObject.Find("Minimap").transform, false);
            runnerIcon.GetComponent<RectTransform>().localScale = new Vector2((cellSize/2) * 1.2f, (cellSize/2) * 1.2f);
            runnerIcon.name = "RunIcon";
            runnerIcon.gameObject.SetActive(false);
        }

        // If the user is playing as the runner, intitialize the runner's mini map icon
        else{
            runnerIcon = Instantiate(runnerIconPrefab, transform);
            runnerIcon.transform.SetParent(GameObject.Find("Minimap").transform, false);
            runnerIcon.GetComponent<RectTransform>().localScale = new Vector2((cellSize/2) * 1.2f, (cellSize/2) * 1.2f);
            runnerIcon.name = "RunIcon";
        }

        // Initialize the wait time
        nextSpawnTime = Time.time + waitTime;
    }

    // Update is called once per frame
    void Update(){
        string mapCellName,                   // GameObject name of the minimap cell cooresponding to the character's position
               cellRoofName;                  // GameObject name of the minimap cell roof cooresponding to the runner's position
        bool   runnerIconInitialized = false; // Reflects whether the positon of the runner's minimap icon has been initialized

        if(CustomNetworkManager.PlayerRoleSet == true){

            // Initialize minimap components
            if(componentsInitialized == false){
                InitializeMinimapComponents();
                componentsInitialized = true;
            }

            // If the player is playing as the guard master, update all character locations
            if(CustomNetworkManager.IsRunner == false){
                //Debug.Log("isRunner is false");
                // Update the Chaser Icon's location on the minimap
                newChaserCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.CHASER);
                if((chaserCellLocation[0] != newChaserCellLocation[0]) || (chaserCellLocation[1] != newChaserCellLocation[1])){
                    if(CellLocationIsValid(newChaserCellLocation)){
                        mapCellName = "cf(" + newChaserCellLocation[0] + "," + newChaserCellLocation[1] + ")";
                        chaserIcon.GetComponent<RectTransform>().localPosition = GameObject.Find(mapCellName).GetComponent<RectTransform>().localPosition;
                        chaserCellLocation[0] = newChaserCellLocation[0];
                        chaserCellLocation[1] = newChaserCellLocation[1];
                    }
                }

                // Update the Engineer Icon's location on the minimap
                newEngineerCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.ENGINEER);
                if((engineerCellLocation[0] != newEngineerCellLocation[0]) || (engineerCellLocation[1] != newEngineerCellLocation[1])){
                    if(CellLocationIsValid(newEngineerCellLocation)){
                        mapCellName = "cf(" + newEngineerCellLocation[0] + "," + newEngineerCellLocation[1] + ")";
                        engineerIcon.GetComponent<RectTransform>().localPosition = GameObject.Find(mapCellName).GetComponent<RectTransform>().localPosition;
                        engineerCellLocation[0] = newEngineerCellLocation[0];
                        engineerCellLocation[1] = newEngineerCellLocation[1];
                    }
                }

                // Update the Trapper Icon's location on the minimap
                newTrapperCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.TRAPPER);
                if((trapperCellLocation[0] != newTrapperCellLocation[0]) || (trapperCellLocation[1] != newTrapperCellLocation[1])){
                    if(CellLocationIsValid(newTrapperCellLocation)){
                        mapCellName = "cf(" + newTrapperCellLocation[0] + "," + newTrapperCellLocation[1] + ")";
                        trapperIcon.GetComponent<RectTransform>().localPosition = GameObject.Find(mapCellName).GetComponent<RectTransform>().localPosition;
                        trapperCellLocation[0] = newTrapperCellLocation[0];
                        trapperCellLocation[1] = newTrapperCellLocation[1];
                    }
                }

                // Update the Runner Icon's location on the minimap
                newRunnerCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.RUNNER);
                if(((runnerCellLocation[0] != newRunnerCellLocation[0]) || (runnerCellLocation[1] != newRunnerCellLocation[1])) || runnerIconInitialized == false){
                    if(CellLocationIsValid(newRunnerCellLocation)){
                        mapCellName  = "cf(" + newRunnerCellLocation[0] + "," + newRunnerCellLocation[1] + ")";
                        cellRoofName = "cr(" + newRunnerCellLocation[0] + "," + newRunnerCellLocation[1] + ")";
                        runnerIcon.GetComponent<RectTransform>().localPosition = GameObject.Find(mapCellName).GetComponent<RectTransform>().localPosition;
                        Destroy(GameObject.Find(cellRoofName));
                        runnerCellLocation[0] = newRunnerCellLocation[0];
                        runnerCellLocation[1] = newRunnerCellLocation[1];
                        runnerIconInitialized = true;
                    }
                }

                // Flash the runner minimap icon on and off
                if(Time.time > nextSpawnTime){
                    nextSpawnTime += waitTime;
                    if(runnerIcon.GetComponent<SpriteRenderer>().enabled == true){
                        runnerIcon.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    else{
                        runnerIcon.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }

            // If the player is playing as the runner, update the runner icon location
            else{
                //Debug.Log("isRunner is true");
                newRunnerCellLocation = Utilities.GetCharacterCellLocation(ManageActiveCharactersConstants.RUNNER);
                if(((runnerCellLocation[0] != newRunnerCellLocation[0]) || (runnerCellLocation[1] != newRunnerCellLocation[1])) || runnerIconInitialized == false){
                    if(CellLocationIsValid(newRunnerCellLocation)){
                        mapCellName  = "cf(" + newRunnerCellLocation[0] + "," + newRunnerCellLocation[1] + ")";
                        cellRoofName = "cr(" + newRunnerCellLocation[0] + "," + newRunnerCellLocation[1] + ")";
                        runnerIcon.GetComponent<RectTransform>().localPosition = GameObject.Find(mapCellName).GetComponent<RectTransform>().localPosition;
                        Destroy(GameObject.Find(cellRoofName));
                        runnerCellLocation[0] = newRunnerCellLocation[0];
                        runnerCellLocation[1] = newRunnerCellLocation[1];
                        runnerIconInitialized = true;
                    }
                }
            }
        }
    }

    // Determine if a cell exists for the given cell location
    private bool CellLocationIsValid(int[] cellLocation){
        if((cellLocation[0] > -Utilities.GetMazeWidth()/2 && cellLocation[0] < Utilities.GetMazeWidth()/2) && (cellLocation[1] > -Utilities.GetMazeHeight()/2 && cellLocation[1] < Utilities.GetMazeHeight()/2)){
            return true;
        }
        else{
            return false;
        }
    }

    // Start the coroutine to display the alert letting the guard master know that the runner has been detected
    public void ProcessTrapChestTriggeredAlert(){
        if(!CustomNetworkManager.IsRunner){
            StartCoroutine(DisplayTrapChestTriggeredAlert());
        }
    }

    // Display the alert letting the guard master know that the runner has been detected
    IEnumerator DisplayTrapChestTriggeredAlert(){
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(true);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(true);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(true);
        yield return new WaitForSeconds(4);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertBackground")).SetActive(false);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("alertText")).SetActive(false);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RunIcon")).SetActive(false);
    }
}
