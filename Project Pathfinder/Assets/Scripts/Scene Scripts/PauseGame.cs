using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public GameObject PauseCanvas;

    public void Awake()
    {
        PauseCanvas = GameObject.Find("PauseCanvas");
        PauseCanvas.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            PauseCanvas.gameObject.SetActive(!PauseCanvas.gameObject.activeSelf);
        }
    }

    public void OpenPauseCanvas()
    {
        PauseCanvas.SetActive(true);
    }

    public void ClosePauseCanvas()
    {
        PauseCanvas.SetActive(false);
    }

    public void ContinueGame(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ExitGame(int index)
    {
        Application.Quit();
        SceneManager.LoadScene(index);
    }
}
