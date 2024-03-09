using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayUnlockSound : MonoBehaviour
{
    public AudioSource unlockSound;

    // Plays the unlock sound
    void Start()
    {
        unlockSound.Play();

        StartCoroutine(coroutine_PlayGuardAlarm());
    }

    // Plays the Guardmaster alarm sound
    IEnumerator coroutine_PlayGuardAlarm()
    {
        var alarm = GameObject.Find("GuardAlarmSound").GetComponent<AudioSource>();
        if(CustomNetworkManager.isRunner == false && alarm.isPlaying == false)
        {
            alarm.Play();
            Debug.Log("CommandManager: Alarm is playing");
            yield return new WaitForSeconds(2);
            Debug.Log("CommandManager: Alarm is no longer playing");
            alarm.Stop();
        }
    }
}
