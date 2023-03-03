using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManageMinimapClicks : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesHitTriggers = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Initiate guard fast travel if the minimap floor parent object is clicked
    void OnMouseDown()
    {
        if(!CustomNetworkManager.isRunner)
        {
            var activeGuardId = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<ManageActiveCharacters>().activeGuardId;
            var mazeCell = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("mcf" + gameObject.name.Substring(2)));
            ManageFastTravel.FastTravel(activeGuardId, mazeCell.transform.position);
        }
    }

    void OnMouseOver(){
        var activeGuardId = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Chaser")).GetComponent<ManageActiveCharacters>().activeGuardId;

        if(activeGuardId == ManageActiveCharactersConstants.CHASER){
            spriteRenderer.color = Color.green;
        }
        else if(activeGuardId == ManageActiveCharactersConstants.ENGINEER){
            spriteRenderer.color = Color.yellow;
        }
        else{
            spriteRenderer.color = Color.blue;
        }        
    }

    void OnMouseExit(){
        spriteRenderer.color = Color.white;
    }
}
