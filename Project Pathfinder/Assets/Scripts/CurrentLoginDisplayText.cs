using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLoginDisplayText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string username = CustomNetworkManager.currentLogin.username;
        GetComponent<TMPro.TMP_Text>().text = "Currently logged in as " + username +
            ".\nNot you? Login here!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
