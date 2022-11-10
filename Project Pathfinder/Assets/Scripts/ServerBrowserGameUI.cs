using UnityEngine;

public class ServerBrowserGameUI : MonoBehaviour
{
    public static ServerBrowserGameUI Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Buttons
    public void OnTutorialButton()
    {
        Debug.Log("OnTutorialButton");
    }
    public void OnOnlineButton()
    {
        Debug.Log("OnOnlineButton");
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
