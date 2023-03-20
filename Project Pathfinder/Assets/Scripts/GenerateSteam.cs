using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenerateSteam : MonoBehaviour
{
    public static float steam = 0;   // Amount of steam available for use
    public int displaySteam;         // Amount of steam displayed
    private float steamLimit = 101f; // Total steam limit
    private float steamMultiplier;   // Variable steam generation
    public int generatorCount = 3; // Inital number of generators

    // Update is called once per frame
    void Update()
    {
        // Generate steam
        if(steam < steamLimit){
            displaySteam = (int)steam;
            gameObject.GetComponent<TextMeshProUGUI>().text = displaySteam.ToString();
            switch(generatorCount){
                case 0: 
                    steamMultiplier = 0f;
                    break;
                case 1:
                    steamMultiplier = 0.33f;
                    break;
                case 2:
                    steamMultiplier = 0.66f;
                    break;
                case 3:
                    steamMultiplier = 1f;
                    break;
                default:
                    steamMultiplier = 1f;
                    break;
            }
            steam += Time.deltaTime * steamMultiplier;
        }
    }
}
