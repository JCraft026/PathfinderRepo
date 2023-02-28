using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagePopups : MonoBehaviour
{
    public TMP_Text popupText; // Text to display on the popup

    // Display a popup at the top of the canvas
    public IEnumerator DisplayPopup(string text, int time){
        popupText.text = text;
        yield return new WaitForSecondsRealtime(time);
        popupText.text = "woop";
    }
}
