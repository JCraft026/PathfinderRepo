using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageCrackedPopup : MonoBehaviour
{
    public GameObject popUpLeft;  // Popup message to the left of the LR cracked wall
    public GameObject popUpRight; // Popup message to the right of the LR cracked wall
    public GameObject popUp;      // Popup message on the TB cracked wall
    private Player_UI playerUi;   // Player UI management script
    Regex lrWallExpression = new Regex("Wall_LR"); 
                                  // Match left and right walls

    // Start is called before the first frame update
    void Start()
    {
        // Assign Player UI script
        playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();
    }

    // Update is called once per frame
    void Update()
    {
        var runnerPosition = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).transform.position;
                                                                                            // Position of the runner game object
        
        // If the runner is close to a cracked wall and is holding the sledgehammer, didplay the popup
        if(CustomNetworkManager.isRunner && Utilities.GetDistanceBetweenObjects(gameObject.transform.position, runnerPosition) <= 2f && playerUi.activeSelectedItem == Item.ItemType.Sledge){
            if(lrWallExpression.IsMatch(gameObject.name)){
                if(runnerPosition.x > gameObject.transform.position.x){
                    popUpLeft.SetActive(true);
                }
                else{
                    popUpRight.SetActive(true);
                }
            }
            else{
                popUp.SetActive(true);
            }
        }
        else{
            if(lrWallExpression.IsMatch(gameObject.name)){
                popUpLeft.SetActive(false);
                popUpRight.SetActive(false);
            }
            else{
                popUp.SetActive(false);
            }
        }
    }
}
