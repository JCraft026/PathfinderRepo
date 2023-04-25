using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GuardAbilityBase : NetworkBehaviour
{
    public AudioSource audioSource;


    [Command]
    public void cmd_PlaySyncedAbilityAudio()
    {
        rpc_PlaySyncedAbilityAudio();
    }

    [ClientRpc]
    public void rpc_PlaySyncedAbilityAudio()
    {
        if(audioSource.isPlaying == false)
            audioSource.Play();
        else
            Debug.Log(gameObject.name + " ability audio source is already playing");
    }
}
