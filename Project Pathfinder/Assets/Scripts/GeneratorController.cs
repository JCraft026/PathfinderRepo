using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneratorController : MonoBehaviour
{
    public Animator animator;
    public GameObject player {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        }
        set{}
    }


    void Update(){
        if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam < 101 && animator.GetBool("IsBusted") == false){
            GeneratorStart();
        }
        else if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam >= 101 && animator.GetBool("IsBusted") == false){
            GeneratorStop();
        }
    }

     public static void breakGenerator(){
        Debug.Log("GeneratorController: breakGenerator called");
        GameObject generator = GeneratorController.FindGeneratorClosestToRunner();

        if(Utilities.GetDistanceBetweenObjects(generator.transform.position, generator.GetComponent<GeneratorController>().player.transform.position) < 1.2f){
            Debug.Log("GeneratorController: distance between runner & generator fine. Generator should break");
            generator.GetComponent<GeneratorController>().GeneratorBreak();  
        }

        Debug.Log("GeneratorController breakGenerator(): Distance between runner and generator is:" + Utilities.GetDistanceBetweenObjects(generator.transform.position, generator.GetComponent<GeneratorController>().player.transform.position).ToString());
    }

    public void GeneratorBreak()
    {
        if(animator.GetBool("IsBusted") == false)
        {
            animator.SetBool("IsBusted", true);
            GenerateSteam.generatorCount -= 1;
            Debug.Log("GeneratorController: Generator broken");
        }
        else
        {
            Debug.Log("GeneratorController: Generator already broken");
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

    // Find the generator closest to the runner
    private static GameObject FindGeneratorClosestToRunner()
    {
        GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name.Contains("Runner"));
        List<GameObject> allGenerators = Resources.FindObjectsOfTypeAll<GameObject>()
                                            .Where<GameObject>(x => 
                                                x.GetComponent<GeneratorController>() != null)
                                            .ToList();
        double distance = double.MaxValue;
        GameObject closestGenerator = allGenerators[0];
        for(int i = 0; i < allGenerators.Count; i++)
        {
            double calcedDist = Utilities.GetDistanceBetweenObjects(runner.transform.position, allGenerators[i].transform.position);
            if(calcedDist < distance)
            {
                distance = calcedDist;
                closestGenerator = allGenerators[i];
            }
        }
        return closestGenerator;
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
