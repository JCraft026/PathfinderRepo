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
        spriteRenderer.color = Color.yellow;
    }

    void OnMouseExit(){
        spriteRenderer.color = Color.white;
    }
}
