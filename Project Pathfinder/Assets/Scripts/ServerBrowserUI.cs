using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;

public class ServerBrowserUI : MonoBehaviour
{
    public static ServerBrowserUI Instance { set; get; }

    [SerializeField] private Animator menuAnimator;

    private void Awake()
    {
        Instance = this;
    }

    // Buttons
    public void OnTutorialButton()
    {

    }
    public void OnPlayButton()
    {

    }

    public void OnSettingsButton()
    {

    }

    public void OnOnlineHostButton()
    {
 
    }

    public void OnOnlineConnectButton()
    {

    }

    public void OnOnlineBackButton()
    {

    }

    public void OnHostCancelButton()
    {

    }
}
