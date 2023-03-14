using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageCharacterLayers : MonoBehaviour
{
    public Regex runnerExpression   = new Regex("Runner");   // Match "Runner"
    public Regex chaserExpression   = new Regex("Chaser");   // Match "Chaser"
    public Regex engineerExpression = new Regex("Engineer"); // Match "Engineer"
    public Regex trapperExpression  = new Regex("Trapper");  // Match "Trapper"
    private GameObject runnerArrow;                          // Runner arrow game object
    private GameObject chaserArrow;                          // Chaser arrow game object
    private GameObject engineerArrow;                        // Engineer arrow game object
    private GameObject trapperArrow;                         // Trapper arrow game object

    /*
    void Start(){
        // Assign arrow game objects
        runnerArrow   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Red Arrow"));
        chaserArrow   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Green Arrow"));
        engineerArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Gold Arrow"));
        trapperArrow  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Blue Arrow"));
    }
    */

    // Update is called once per frame
    void Update()
    {
        // Assign arrow game objects
        runnerArrow   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Red Arrow"));
        chaserArrow   = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Green Arrow"));
        engineerArrow = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Gold Arrow"));
        trapperArrow  = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Blue Arrow"));

        // Set character order in layer based on its y value rank
        if(runnerExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder    = HandleLayers.runnerElevationRank;
            runnerArrow.GetComponent<SpriteRenderer>().sortingOrder   = HandleLayers.runnerElevationRank;
        }
        else if(chaserExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder    = HandleLayers.chaserElevationRank;
            chaserArrow.GetComponent<SpriteRenderer>().sortingOrder   = HandleLayers.chaserElevationRank;
        }
        else if(engineerExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder    = HandleLayers.engineerElevationRank;
            engineerArrow.GetComponent<SpriteRenderer>().sortingOrder = HandleLayers.engineerElevationRank;
        }
        else if(trapperExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder    = HandleLayers.trapperElevationRank;
            trapperArrow.GetComponent<SpriteRenderer>().sortingOrder  = HandleLayers.trapperElevationRank;
        }
    }
}
