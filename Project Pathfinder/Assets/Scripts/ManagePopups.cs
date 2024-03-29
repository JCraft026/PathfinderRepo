using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ManagePopups : MonoBehaviour
{
    public TMP_Text popupText;        // Text to display on the popup
    public TMP_Text abilityErrorText; // Text to display on the popup

    // Process received popup display request
    public void ProcessPopup(string text, float time){
        StartCoroutine(DisplayPopup(text, time));
    }

    // Display a popup at the top of the canvas
    IEnumerator DisplayPopup(string text, float time){
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("popupBackground")).SetActive(true);
        popupText.text = text;
        yield return new WaitForSeconds(time);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("popupBackground")).SetActive(false);
        popupText.text = "";
    }

    // Process received ability alert display request
    public void ProcessAbilityAlert(string text, float time){
        StartCoroutine(DisplayAbilityAlert(text, time));
    }

    // Display ability alert at the top of the canvas
    IEnumerator DisplayAbilityAlert(string text, float time){
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("abilityErrorBackground")).SetActive(true);
        abilityErrorText.text = text;
        yield return new WaitForSeconds(time);
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("abilityErrorBackground")).SetActive(false);
        abilityErrorText.text = "";
    }
}
