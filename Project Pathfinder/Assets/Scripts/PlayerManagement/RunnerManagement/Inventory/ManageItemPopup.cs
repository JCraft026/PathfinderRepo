using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Linq;

public class ManageItemPopup : MonoBehaviour
{
    public GameObject popUp;
    public bool itemIsSelected = false;
    public bool alreadyDisplayed = false;

    void Start(){
        popUp.SetActive(false);
    }

    void Update(){
        // Diplay item description if it is the selected item
        if(itemIsSelected && !alreadyDisplayed){
            StartCoroutine(DisplayItemPopup());
            itemIsSelected = false;
        }
    }

    IEnumerator DisplayItemPopup(){
        popUp.SetActive(true);
        yield return new WaitForSeconds(5);
        popUp.SetActive(false);
    }
}
