using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneratorController : MonoBehaviour
{
    public Animator animator;
    private GameObject player;

    void Awake(){
        GameObject player = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
    }


    void Update(){
        if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam < 101 && animator.GetBool("IsBusted") == false){
            GeneratorStart();
        }
        else if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam >= 101 && animator.GetBool("IsBusted") == false){
            GeneratorStop();
        }
    }

     public void breakGenerator(){
        if(Utilities.GetDistanceBetweenObjects(transform.position, player.transform.position) < 1.2f){
            GeneratorBreak();  
        }
    }

    public void GeneratorBreak()
    {
        if(animator.GetBool("IsBusted") == false)
        {
            animator.SetBool("IsBusted", true);
            GenerateSteam.generatorCount -= 1;
        }
    }

    // Anne's new version
    public void GeneratorFixed()
    {
        if(animator.GetBool("IsBusted") == true)
        {
            animator.SetBool("IsBusted", false);
            GenerateSteam.generatorCount += 1;
        }
    }

    public void GeneratorStart()
    {
        animator.SetBool("IsGenerating", true);
    }

    public void GeneratorStop()
    {
        animator.SetBool("IsGenerating", false);
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
}
