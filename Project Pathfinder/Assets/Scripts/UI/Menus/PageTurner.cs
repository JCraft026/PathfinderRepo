using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTurner : MonoBehaviour
{
    public GameObject RunnerPage;
    public GameObject EscapeProcessPage;
    public GameObject ItemPage;
    public GameObject GeneralGuardPage;
    public GameObject TrapperPage;
    public GameObject ChaserPage;
    public GameObject EngineerPage;

    public void Awake()
    {
        RunnerPage        = GameObject.Find("RunnerPage");
        EscapeProcessPage = GameObject.Find("EscapeProcessPage");
        ItemPage          = GameObject.Find("ItemPage");
        GeneralGuardPage  = GameObject.Find("GeneralGuardPage");
        TrapperPage       = GameObject.Find("TrapperPage");
        ChaserPage        = GameObject.Find("ChaserPage");
        EngineerPage      = GameObject.Find("EngineerPage");
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

    public void TurnGuardmasterPageBack()
    {
        ItemPage.SetActive(true);
    }

    public void TurnGuardmasterPageForward()
    {
        GeneralGuardPage.SetActive(false);
    }

    public void TurnTrapperPageBack()
    {
        GeneralGuardPage.SetActive(true);
    }
    public void TurnTrapperPageForward()
    {
        TrapperPage.SetActive(false);
    }

    public void TurnChaserPageBack()
    {
        TrapperPage.SetActive(true);
    }
    public void TurnChaserPageForward()
    {
        ChaserPage.SetActive(false);
    }

    public void TurnEngineerPageBack()
    {
        ChaserPage.SetActive(true);
    }
}
