using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenuButton : MonoBehaviour
{
    public GameObject MainMenuButton;

    void FixedUpdate(){
        if(MainMenuButton.activeSelf == false)
        {
            MainMenuButton.SetActive(true);
        }
    }
}
