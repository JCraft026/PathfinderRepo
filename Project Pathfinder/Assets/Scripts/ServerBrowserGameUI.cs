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
}
