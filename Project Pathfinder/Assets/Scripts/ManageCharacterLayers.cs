using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ManageCharacterLayers : MonoBehaviour
{
    public Regex runnerExpression = new Regex("Runner"); // Match "Runner"
    public Regex chaserExpression = new Regex("Chaser"); // Match "Chaser"
    public Regex engineerExpression = new Regex("Engineer"); // Match "Engineer"
    public Regex trapperExpression = new Regex("Trapper"); // Match "Trapper"

    // Update is called once per frame
    void Update()
    {
        // Set character order in layer based on its y value rank
        /*
        if(runnerExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = HandleLayers.runnerElevationRank;
        }
        */
        if(chaserExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = HandleLayers.chaserElevationRank;
        }
        else if(engineerExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = HandleLayers.engineerElevationRank;
        }
        else if(trapperExpression.IsMatch(gameObject.name)){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = HandleLayers.trapperElevationRank;
        }
    }
}
