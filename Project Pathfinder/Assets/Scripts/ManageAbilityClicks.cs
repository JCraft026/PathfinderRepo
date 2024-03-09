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
            GameObject.Find("Chaser(Clone)").GetComponent<ChaserAbility>().AbilityClicked = true;
        }
        else if(engineerAbilityExpression.IsMatch(gameObject.name)){
            GameObject.Find("Engineer(Clone)").GetComponent<EngineerAbility>().AbilityClicked = true;
        }
        else if(trapperAbilityExpression.IsMatch(gameObject.name)){
            GameObject.Find("Trapper(Clone)").GetComponent<TrapperAbility>().AbilityClicked = true;
        }
    }

    void OnMouseExit(){
        popUp.SetActive(false);
    }
}
