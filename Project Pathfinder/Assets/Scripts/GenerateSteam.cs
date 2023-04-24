using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class GenerateSteam : MonoBehaviour
{
    public static float steam = 0;   // Amount of steam available for use
    public int displaySteam;         // Amount of steam displayed
    private float steamLimit = 101f; // Total steam limit
    private float steamMultiplier;   // Variable steam generation
    const int GENERATOR_COUNT = 3; // Inital number of generators
    private int toSubtract = 0;
    public Image steamBarImage; //

    public Sprite steamBar0;
    public Sprite steamBar10;
    public Sprite steamBar20;
    public Sprite steamBar30;
    public Sprite steamBar40;
    public Sprite steamBar50;
    public Sprite steamBar60;
    public Sprite steamBar70;
    public Sprite steamBar80;
    public Sprite steamBar90;
    public Sprite steamBar100;


    private void Start()
    {
        steamBarImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // Generate steam
        if(steam < steamLimit && CustomNetworkManager.steamGeneratorsSpawned
            && ((CustomNetworkManager.isHost && CustomNetworkManager.clientJoined) // If we are hosting we MUST wait for the client to join in order to generate steam
            || CustomNetworkManager.isHost == false) ){ // If we are not hosting we can reasonably assume that the host is already in the game so we can generate steam
            displaySteam = (int)steam;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = displaySteam.ToString();
            switch(GENERATOR_COUNT - subtractBrokenGenerators()){
                case 0: 
                    steamMultiplier = 0.25f;
                    break;
                case 1:
                    steamMultiplier = 0.5f;
                    break;
                case 2:
                    steamMultiplier = 0.75f;
                    break;
                case 3:
                    steamMultiplier = 1f;
                    break;
                default:
                    steamMultiplier = 1f;
                    break;
            }
            steam += Time.deltaTime * steamMultiplier;

            switch((int) steam)
            {
                case 0:
                    steamBarImage.sprite = steamBar0;
                    break;
                case int n when (n < 20):
                    steamBarImage.sprite = steamBar10;
                    break;
                case int n when (n >= 20 && n < 30):
                    steamBarImage.sprite = steamBar20;
                    break;
                case int n when (n >= 30 && n < 40):
                    steamBarImage.sprite = steamBar30;
                    break;
                case int n when (n >= 40 && n < 50):
                    steamBarImage.sprite = steamBar40;
                    break;
                case int n when (n >= 50 && n < 60):
                    steamBarImage.sprite = steamBar50;
                    break;
                case int n when (n >= 60 && n < 70):
                    steamBarImage.sprite = steamBar60;
                    break;
                case int n when (n >= 70 && n < 80):
                    steamBarImage.sprite = steamBar70;
                    break;
                case int n when (n >= 80 && n < 90):
                    steamBarImage.sprite = steamBar80;
                    break;
                case int n when (n >= 90 && n < 100):
                    steamBarImage.sprite = steamBar90;
                    break;
                case 100:
                    steamBarImage.sprite = steamBar100;
                    break;
            }
        }
    }

    public int subtractBrokenGenerators(){
        toSubtract = 0;
        List<GameObject> allGenerators = Resources.FindObjectsOfTypeAll<GameObject>().Where<GameObject>(x => x.GetComponent<GeneratorController>() != null).ToList();
        allGenerators.ForEach(x =>{ 
            if(x.GetComponent<Animator>().GetBool("IsBusted") == true) 
                toSubtract += 1;
        });
        return toSubtract;
    }
}
