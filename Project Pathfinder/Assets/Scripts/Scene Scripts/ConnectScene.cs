using UnityEngine;

public class ConnectScene : MonoBehaviour
{
    public GameObject HostCanvas;

    public void Awake()
    {
        HostCanvas = GameObject.Find("HostMenu");
    }

    public void ShowHostMenu()
    {
        
    }
}
