using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUnlockSound : MonoBehaviour
{
    public AudioSource unlockSound;

    // Plays the unlock sound when the KeyHole (Unlocked) gameobject is enabled
    void Start()
    {
        unlockSound.Play();
    }
}
