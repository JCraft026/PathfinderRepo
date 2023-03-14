using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenerateSteam : MonoBehaviour
{
    public static float steam;       // Amount of steam available for use
    public int displaySteam;         // Amount of steam displayed
    private float steamLimit = 101f; // Total steam limit

    public int generatorCount = 3;   // Starting number of Generators

    // Update is called once per frame
    void Update()
    {
        // Generate steam
        if(steam < steamLimit){
            displaySteam = (int)steam;
            gameObject.GetComponent<TextMeshProUGUI>().text = displaySteam.ToString();
            switch(generatorCount){
                case 0: 
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            steam += Time.deltaTime;
        }
    }
}
