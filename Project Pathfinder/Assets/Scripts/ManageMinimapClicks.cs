using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageMinimapClicks : MonoBehaviour
{
    SpriteRenderer spriteRenderer; // Sprite renderer of the minimap cell parent object

    // Start is called before the first frame update
    void Start()
    {
        // Allow collider trigger interaction with queries
        Physics2D.queriesHitTriggers = true;

        // Assign the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            Debug.LogError("ManageMiniMapClicks: sprite renderer null in start");
        }
    }

    // Initiate guard fast travel if the minimap floor parent object is clicked
    void OnMouseDown()
    {
        if(!CustomNetworkManager.isRunner && GenerateSteam.steam >= 75f){
            int activeGuardId = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)")).GetComponent<ManageActiveCharacters>().activeGuardId;
                                    // Guard ID of the current active guard
            GameObject guard  = null; // Active guard object

            // Subtract from steam
            GenerateSteam.steam -= 75f;

            if(activeGuardId == ManageActiveCharactersConstants.CHASER){
                guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)"));
            }
            else if(activeGuardId == ManageActiveCharactersConstants.ENGINEER){
                guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer(Clone)"));
            }
            else{
                guard = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Trapper(Clone)"));
            }

            // Initiate guard fast travel if the current guard isnt already fast traveling
            if(!guard.GetComponent<Animator>().GetBool("Fast Travel Started"))
            {
                var mazeCell = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("mcf" + gameObject.name.Substring(2)));
                ManageFastTravel.FastTravel(activeGuardId, mazeCell.transform.position);
            }
        }

    }

    void OnMouseOver(){
        // Change minimap cell color based on the current active guard
        if(!CustomNetworkManager.isRunner){
            var activeGuardId = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser(Clone)")).GetComponent<ManageActiveCharacters>().activeGuardId;
                    // Guard ID of the current active guard

            if(activeGuardId == ManageActiveCharactersConstants.CHASER){
                spriteRenderer.color = Color.green;
            }
            else if(activeGuardId == ManageActiveCharactersConstants.ENGINEER){
                spriteRenderer.color = Color.yellow;
            }
            else{
                spriteRenderer.color = Color.blue;
            }  

            // Display Fast Travel Steam Cost
            Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("FastTravelSteamCost")).SetActive(true);
        }
              
    }

    // Revert minimap cell color
    void OnMouseExit(){
        spriteRenderer.color = Color.white;
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("FastTravelSteamCost")).SetActive(false);
    }
}
