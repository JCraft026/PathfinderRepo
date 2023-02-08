using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PauseGame : NetworkBehaviour
{
    public GameObject PauseCanvas;

    public void Start()
    {
        PauseCanvas.SetActive(false);
    }

    public void Awake()
    {
        //PauseCanvas = GameObject.Find("PauseCanvas");
        PauseCanvas = FindPauseCanvas();
        PauseCanvas.SetActive(true);
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isLocalPlayer)
        {
            Debug.Log(this.gameObject.name + " reacted to escape key");
            //PauseCanvas.gameObject.SetActive(!PauseCanvas.gameObject.activeSelf);
            if(PauseCanvas.gameObject.activeSelf == false)
            {
                OpenPauseCanvas();
            }
            else
            {
                ClosePauseCanvas();
            }

        }
    }

    public GameObject FindPauseCanvas()
    {
       PauseCanvas = SceneManager.GetActiveScene()
                                .GetRootGameObjects()
                                .FirstOrDefault<GameObject>(x => x.name == "PauseCanvas");
        if(PauseCanvas == null)
        {
            throw(new Exception("Could locate exit game menu"));
        }
        else
            return PauseCanvas;
    }

    public void OpenPauseCanvas()
    {
        Debug.Log("Open Exit Menu");
        PauseCanvas.SetActive(true);
    }

    public void ClosePauseCanvas()
    {
        Debug.Log("Close Exit Menu");
        PauseCanvas.SetActive(false);
    }

    public void ContinueGame(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ExitGame(int index)
    {
        NetworkClient.Shutdown();
        NetworkServer.Shutdown();
        Application.Quit();
        SceneManager.LoadScene(index);
    }
}
