using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageTextureLayers : MonoBehaviour
{
    public Regex horizontalWallExpression = new Regex("TopBottom"); // Match "TopBottom"

    // Update is called once per frame
    void Update()
    {   
        // If the game object is below the active character, adjust order in layer accordingly
        if(gameObject.transform.position.y - HandleEvents.activeCharacterLocation.y < 0){
            if(horizontalWallExpression.IsMatch(gameObject.name)){
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
            }
        }
        // If the game object is above the active character, adjust order in layer accordingly
        else{
            if(horizontalWallExpression.IsMatch(gameObject.name)){
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }
}
