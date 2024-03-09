using System;
using System.Collections;
using System.Collections.Generic; // KeyValuePair<,>

using UnityEngine;

public class ConnectScene : MonoBehaviour
{
    public GameObject ConnectCanvas;

    public void Awake()
    {
        ConnectCanvas = GameObject.Find("ConnectCanvas");
    }

    public void ShowHostMenu()
    {
        AddNamesToDropdownMenus();
        ConnectCanvas.SetActive(false);
    }

    public void HideHostMenu()
    {
        ConnectCanvas.SetActive(true);
    }
    
    // Adds the adjectives and titles to the server name selection windows.
    public void AddNamesToDropdownMenus() {
        //PlayerProfile currentProfile = CustomNetworkManager.currentLogin;
        
        GameObject dropdownAdjective1 = GameObject.Find("Dropdown (1)");
        GameObject dropdownAdjective2 = GameObject.Find("Dropdown (3)");
        GameObject dropdownTitle      = GameObject.Find("Dropdown (2)");
        
        List<string> options_adjectives = new List<string>();
        foreach (KeyValuePair<string,bool> adj in CustomNetworkManager.currentLogin.adjectives) {
            if (adj.Value == true)
                options_adjectives.Add(adj.Key);
        }
        List<string> options_titles = new List<string>();
        foreach (KeyValuePair<string,bool> adj in CustomNetworkManager.currentLogin.titles) {
            if (adj.Value == true)
                options_titles.Add(adj.Key);
        }
        
        dropdownAdjective1.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownAdjective1.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_adjectives);
        dropdownAdjective2.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownAdjective2.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_adjectives);
        dropdownTitle.GetComponent<TMPro.TMP_Dropdown>().ClearOptions();
        dropdownTitle.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options_titles);
    }
}
