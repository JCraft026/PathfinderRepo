using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GeneratorController : NetworkBehaviour
{
    public Animator animator;
    public GameObject generator {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("SteamGenerator"));
        }
        set{}
    }
    public GameObject player {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
        }
        set{}
    }
    public GameObject engineer {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Engineer"));
        }
        set{}
    }

    void Update(){
        // Controls turning on and off steam generators when you need to generate
        if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam < 100f && animator.GetBool("IsBusted") == false){
            GeneratorStart();
        }
        else if(animator.GetBool("IsGenerating") == true && GenerateSteam.steam >= 100f && animator.GetBool("IsBusted") == false){
            GeneratorStop();
        }

        // Detects if the Engineer is near to fix the generator
        if(Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 1.5f, generator.transform.position.y -1.5f), generator.GetComponent<GeneratorController>().engineer.transform.position) < 3f){
            Debug.Log("THE ENGINEER IS HERE");
        }
    }

     public static bool breakGenerator(){
        bool generatorBroken = false;
        Debug.Log("GeneratorController: breakGenerator called");
        GameObject generator = GeneratorController.FindGeneratorClosestToRunner();

        if(Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 1.5f, generator.transform.position.y -1.5f), generator.GetComponent<GeneratorController>().player.transform.position) < 1.5f){
            Debug.Log("GeneratorController: distance between runner & generator fine. Generator should break");
            generator.GetComponent<GeneratorController>().GeneratorBreak(); 
            generatorBroken = true; 
        }
        Debug.Log("GeneratorController breakGenerator(): Distance between runner and generator is:" + Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 1.5f, generator.transform.position.y -1.5f), generator.GetComponent<GeneratorController>().player.transform.position).ToString());
        return generatorBroken;    
    }

    public void GeneratorBreak()
    {
        if(animator.GetBool("IsBusted") == false)
        {
            animator.SetBool("IsBusted", true);
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
            Debug.Log("GeneratorController: Generator fixed");
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
}
