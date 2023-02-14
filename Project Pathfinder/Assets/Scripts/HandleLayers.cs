using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleLayers : MonoBehaviour
{
    public static Vector3 activeCharacterLocation; // Scene position of the current active character
    public static int runnerElevationRank;         // Order of elevation the runner is in
    public static int chaserElevationRank;         // Order of elevation the chaser is in
    public static int engineerElevationRank;       // Order of elevation the engineer is in
    public static int trapperElevationRank;        // Order of elevation the trapper is in

    // Update is called once per frame
    void Update()
    {
        // Update the location of the active character
        UpdateActiveCharacterLocation();

        // Update the elevation rank of each character
        UpdateCharacterElevationOrder();
    }

    // Update the scene location of the character the player is controlling
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

    // Update the elevation order of the characters based on their y position
    public void UpdateCharacterElevationOrder(){
        //GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));     // Runner game object
        GameObject chaser = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser"));     // Chaser game object
        GameObject engineer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer")); // Engineer game object
        GameObject trapper = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper"));   // Trapper game object
        //float[] characterPositions = new float[] { runner.transform.position.y, chaser.transform.position.y, engineer.transform.position.y, trapper.transform.position.y };
                                                                                                                    // All characters current y positions
        float tempPosition;                                                                                         // Temporary character position for swapping


                                            float[] characterPositions = new float[] { chaser.transform.position.y, engineer.transform.position.y, trapper.transform.position.y };
        
        // Sort the y positions making the highest y position at index 0
        for (int count = 1; count <= 4; count++)
        {
            for (int index = 0; index < 2; index++)                                  // CHANGE THE LIMIT BACK TO 3
            {
                if(characterPositions[index] < characterPositions[index + 1]){
                    tempPosition                  = characterPositions[index];
                    characterPositions[index]     = characterPositions[index + 1];
                    characterPositions[index + 1] = tempPosition;
                }
            }
            
        }

        // Update the character elevation ranks based on the order of their y positions
        for (int index = 0; index < 2; index++)                                    // CHANGE THE LIMIT BACK TO 3
        {
            /*
            if(characterPositions[index] == runner.transform.position.y){
                runnerElevationRank  = index + 4;
            }
            */
            if(characterPositions[index] == chaser.transform.position.y){
                chaserElevationRank  = index + 4;
            }
            else if(characterPositions[index] == engineer.transform.position.y){
                engineerElevationRank  = index + 4;
            }
            else if(characterPositions[index] == trapper.transform.position.y){
                trapperElevationRank  = index + 4;
            }
        }
    }
}
