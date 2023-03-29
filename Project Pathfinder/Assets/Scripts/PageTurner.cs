using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTurner : MonoBehaviour
{
    public void TurnRunnerPage()
    {
        CanvasRenderer.FindObjectOfType<GameObject>(GameObject.Find("RunnerPage"));
    }

    public void TurnEscapePage()
    {

    }

    public void TurnItemPage()
    {

    }
}
