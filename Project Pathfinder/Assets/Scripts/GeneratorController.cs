using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public bool isBroken = false;
    public Animator animator;


    void Update(){
        if(GenerateSteam.steam >= 101){
            GeneratorStop();
        }
        else if(animator.GetBool("IsBusted") == false){
            GeneratorStart();
        }
    }

    /*public void StartGeneratingSteam()
    {
        if(CustomNetworkManager.isRunner == false)
            StartCoroutine(GenerateSteam());
    }*/

    // Asyncronously generates 1 steam point every second (as long as the generator is not broken)
    /*IEnumerator GenerateSteam()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            if(isBroken == false &&  < 100)
            {
                generatedSteam += 1;
                animator.enabled = true;
            }
            else if(generatedSteam > 100)
            {
                generatedSteam = 100;
                animator.enabled = false;
            }

	        if (generatedSteam == 0)
            {
                GameObject.Find("SteamBar0").SetActive(true);
                GameObject.Find("SteamBar25").SetActive(false);
            }
            else if (generatedSteam == 25)
            {
                GameObject.Find("SteamBar0").SetActive(false);
                GameObject.Find("SteamBar25").SetActive(true);
                GameObject.Find("SteamBar50").SetActive(false);
            }
            else if (generatedSteam == 50)
            {
                GameObject.Find("SteamBar25").SetActive(false);
                GameObject.Find("SteamBar50").SetActive(true);
                GameObject.Find("SteamBar75").SetActive(false);
            }
            else if (generatedSteam == 75)
            {
                GameObject.Find("SteamBar50").SetActive(false);
                GameObject.Find("SteamBar75").SetActive(true);
                GameObject.Find("SteamBar100").SetActive(false);
            }
            else if (generatedSteam == 100)
            {
                GameObject.Find("SteamBar75").SetActive(false);
                GameObject.Find("SteamBar100").SetActive(true);
                // Turn off animation
            }
        }
    }*/

    public bool GeneratorBreak()
    {
        if(isBroken == false)
        {
            isBroken = true;
            animator.SetBool("isBusted", true);
        }
        return isBroken;
    }

    // Anne's new version
    public bool GeneratorFixed()
    {
        if (isBroken == true)
        {
            isBroken = false;
            animator.SetBool("isBusted", false);
        }
        return isBroken;
    }

    public void GeneratorStart()
    {
        animator.SetBool("IsGenerating", true);
    }

    public void GeneratorStop()
    {
        animator.SetBool("IsGenerating", false);
    }
}
