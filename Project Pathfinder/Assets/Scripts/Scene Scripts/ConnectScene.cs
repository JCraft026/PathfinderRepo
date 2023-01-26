using UnityEngine;

public class ConnectScene : MonoBehaviour
{
    public GameObject ConnectCanvas;

    public void Awake()
    {
        ConnectCanvas = GameObject.Find("ConnectCanvas");
    }

    public void ShowHostMenu()
    {
        ConnectCanvas.SetActive(false);
    }

    public void HideHostMenu()
    {
        ConnectCanvas.SetActive(true);
    }
}
