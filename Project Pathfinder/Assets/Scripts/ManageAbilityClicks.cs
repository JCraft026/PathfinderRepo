using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ManageAbilityClicks : MonoBehaviour
{
    public GameObject popUp;                                        // PopUp cooresponding to the parent game object
    Regex chaserAbilityExpression   = new Regex("ChaserAbility");   // Match "ChaserAbility"
    Regex engineerAbilityExpression = new Regex("EngineerAbility"); // Match "EngineerAbility"
    Regex trapperAbilityExpression  = new Regex("TrapperAbility");  // Match "TrapperAbility"

    // Start is called before the first frame update
    void Start()
    {
        // Allow collider trigger interaction with queries
        Physics2D.queriesHitTriggers = true;
    }

    void OnMouseOver(){
        popUp.SetActive(true);
    }

    void OnMouseDown(){
        // Trigger the cooresponding guard abilities
        if(chaserAbilityExpression.IsMatch(gameObject.name)){
            ChaserAbility.abilityClicked   = true;
        }
        else if(engineerAbilityExpression.IsMatch(gameObject.name)){
            EngineerAbility.abilityClicked = true;
        }
        else if(trapperAbilityExpression.IsMatch(gameObject.name)){
            TrapperAbility.abilityClicked  = true;
        }
    }

    void OnMouseExit(){
        popUp.SetActive(false);
    }
}
