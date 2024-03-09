using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static Profile;

public class EndScreenStats : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<TMPro.TMP_Text>().text = gameObject.GetComponent<Profile>().PlayerNewUnlock();
        gameObject.GetComponent<Profile>().SaveEncodedProfile(CustomNetworkManager.CurrentLogin);
    }
}
