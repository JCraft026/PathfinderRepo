using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Mirror;

public class ManageGameTimer : NetworkBehaviour
{
    [SyncVar]
    public float gameTimeLeft = 600f;
    private float minutes;
    private float seconds;

    // Update is called once per frame
    void Update()
    {
        if(!CustomNetworkManager.hostIsFrozen || !CustomNetworkManager.isHost){
            if(gameTimeLeft > 0){
                minutes = Mathf.FloorToInt(gameTimeLeft / 60);
                seconds = Mathf.FloorToInt(gameTimeLeft % 60);
                gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
                gameTimeLeft -= Time.deltaTime;
                Debug.Log("hostIsFrozen, isHost" + (!CustomNetworkManager.hostIsFrozen).ToString() + ", " + (!CustomNetworkManager.isHost).ToString());
            }
            else{
                HandleEvents.endGameEvent = HandleEventsConstants.TIMER_ZERO;
                HandleEvents.currentEvent = HandleEventsConstants.GUARDMASTER_WINS;
            }
        }
    }
}
