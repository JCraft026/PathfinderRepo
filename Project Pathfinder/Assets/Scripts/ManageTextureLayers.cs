using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageTextureLayers : MonoBehaviour
{
    public Regex wallExpression        = new Regex("Wall");         // Match "Wall"
    public Regex exitExpression        = new Regex("Exit");         // Match "Exit"
    public Regex torchExpression       = new Regex("Torch");        // Match "Torch"
    public Regex tunnelExpression      = new Regex("Tunnel");       // Match "Tunnel"
    public Regex controlRoomExpression = new Regex("Control Room"); // Match "Control Room"
    public Regex barricadeExpression   = new Regex("Barricade");    // Match "Barricade"

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "Steam"){
            if((gameObject.transform.position.y - 2f) - HandleLayers.activeCharacterLocation.y < 0){
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 12;
            }
            else{
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            }
        }
        else{
            // If the game object is below the active character, adjust order in layer accordingly
            if((gameObject.transform.position.y - 1.12f) - HandleLayers.activeCharacterLocation.y < 0){
                if(gameObject.tag == "Torch"){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
                }
                else if(tunnelExpression.IsMatch(gameObject.name) || controlRoomExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 12;
                }
                else if(wallExpression.IsMatch(gameObject.name) || barricadeExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 9;
                }
                else if(exitExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 11;
                }
            }
            // If the game object is above the active character, adjust order in layer accordingly
            else{
                if(gameObject.tag == "Torch"){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
                else if(tunnelExpression.IsMatch(gameObject.name) || controlRoomExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
                }
                else if(wallExpression.IsMatch(gameObject.name) || barricadeExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                else if(exitExpression.IsMatch(gameObject.name)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
                }
            }
        }
    }
}
