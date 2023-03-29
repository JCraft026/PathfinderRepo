using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTurner : MonoBehaviour
{
    public GameObject RunnerPage;
    public GameObject EscapeProcessPage;
    public GameObject ItemPage;
    public GameObject GeneralGuardPage;

    public void Awake()
    {
        RunnerPage             = GameObject.Find("RunnerPage");
        EscapeProcessPage      = GameObject.Find("EscapeProcessPage");
        ItemPage               = GameObject.Find("ItemPage");
        GeneralGuardPage       = GameObject.Find("GeneralGuardPage");
    }

    public void TurnRunnerPageForward()
    {
        RunnerPage.SetActive(false);
    }

    public void TurnEscapePageBack()
    {
        RunnerPage.SetActive(true);
    }

    public void TurnEscapePageForward()
    {
        EscapeProcessPage.SetActive(false);
    }

    public void TurnItemPageBack()
    {
        EscapeProcessPage.SetActive(true);
    }
    public void TurnItemPageForward()
    {
        ItemPage.SetActive(false);
    }
}
