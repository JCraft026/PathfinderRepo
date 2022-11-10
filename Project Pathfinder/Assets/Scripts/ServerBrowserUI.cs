using UnityEngine;

public class ServerBrowserUI : MonoBehaviour
{
    public static ServerBrowserUI Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Buttons
    public void OnTutorialButton()
    {
        Debug.Log("OnTutorialButton");
    }
    public void OnPlayButton()
    {
        Debug.Log("OnPlayButton");
    }

    public void OnSettingsButton()
    {
        Debug.Log("OnSettingsButton");
    }

    public void OnOnlineHostButton()
    {
        Debug.Log("OnOnlineHostButton");
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("OnOnlineConnectButton");
    }

    public void OnOnlineBackButton()
    {
        Debug.Log("OnOnlineBackButton");
    }

    public void OnHostCancelButton()
    {
        Debug.Log("OnHostCancelButton");
    }
}
