using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using TMPro;

public class GeneratorController : NetworkBehaviour
{
    public Animator animator;
    
    // Property for generators
    public GameObject generator;
    public GameObject repairUI;
    public int repairCount = 0;
    // Property for player
    public GameObject player {
        get
        {
            return GameObject.Find("Runner");
        }
    }
    // Property for engineer
    public GameObject engineer {
        get
        {
            return GameObject.Find("Engineer(Clone)");
        }
    }

    public GameObject repairingEffect {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("RepairingEffect"));
        }
    }

    void Update(){
        // Controls turning on and off steam generators when you need to generate
        if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam < 100f && animator.GetBool("IsBusted") == false){
            animator.SetBool("IsGenerating", true);
        }
        else if(animator.GetBool("IsGenerating") == true && GenerateSteam.steam >= 100f && animator.GetBool("IsBusted") == false){
            animator.SetBool("IsGenerating", false);
        }

        // Detects if the Engineer is near to fix the generator
        if(animator.GetBool("IsBusted") == true && Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 1.25f, gameObject.transform.position.y -1.25f), engineer.transform.position) < 3f){
            if(repairCount < 60){
                repairCount += 1;
                // Only sets the repairing UI to true
                if(engineer.transform.GetChild(2).gameObject.activeSelf == false){
                    cmd_objectEnable("RepairUI", true);
                    cmd_objectEnable("RepairingEffect", true);
                    // engineer.transform.GetChild(2).gameObject.SetActive(true);
                    // gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    Debug.Log("Set engineer repair (" + engineer.transform.GetChild(2).gameObject + ") to active");
                    Debug.Log("Set generator repair (" + gameObject.transform.GetChild(0).gameObject + ") to active");
                }
                if(repairCount >= 60){
                    cmd_objectEnable("RepairUI", false);
                    cmd_objectEnable("RepairingEffect", false);
                    // engineer.transform.GetChild(2).gameObject.SetActive(false);
                    // gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    cmd_SetSteam("IsBusted", false);
                    repairCount = 0;
                }
            }
        }
        // Turn off repairing UI if away from Generator
        else if(animator.GetBool("IsBusted") == true && Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 1.25f, gameObject.transform.position.y -1.25f), engineer.transform.position) >= 3f){
            if(engineer.transform.GetChild(2).gameObject.activeSelf){
                cmd_objectEnable("RepairingEffect", false);
                // engineer.transform.GetChild(2).gameObject.SetActive(false);
            }
            if(gameObject.transform.GetChild(0).gameObject.activeSelf){
                cmd_objectEnable("RepairUI", false);
                // gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    // Detect if the player can break the generator
    public static bool breakGenerator(){
        bool generatorBroken = false;
        Debug.Log("GeneratorController: breakGenerator called");
        GameObject generator = GeneratorController.FindClosestGenerator("Runner");

        if(generator.GetComponent<Animator>().GetBool("IsBusted") == false && Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 1f, generator.transform.position.y -1f), generator.GetComponent<GeneratorController>().player.transform.position) < 2.0f){
            Debug.Log("GeneratorController: distance between runner & generator fine. Generator should break");
            generator.GetComponent<GeneratorController>().cmd_SetSteam("IsBusted", true); 
            generatorBroken = true; 
        }
        Debug.Log("GeneratorController breakGenerator(): Distance between runner and generator is:" + Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 1f, generator.transform.position.y -1f), generator.GetComponent<GeneratorController>().player.transform.position).ToString());
        return generatorBroken;    
    }

    // Find the generator closest to the runner
    private static GameObject FindClosestGenerator(string target)
    {
        GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name.Contains(target));
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

    void local_objectEnable(string target, bool setting){
        Debug.Log("Called locally to set object");
        if(target == "RepairingEffect"){
            if(repairingEffect == null){
                Debug.LogWarning("REPAIRING EFFECT IS NULL");
            }
            else{
                repairingEffect.SetActive(setting);
            }
        }
        else if(target == "RepairUI"){
            if(repairUI == null){
                Debug.LogWarning("REPAIR UI IS NULL");
            }
            else{
                repairUI.SetActive(setting);
            }
        }
    }

    // Call an RPC to set steam booleans true or false
    [Command(requiresAuthority = false)]
    void cmd_SetSteam(string parameterToSet, bool setting){
        rpc_SetSteam(parameterToSet, setting);
        Debug.Log("Called Command to set steam");
    }

    // Set steam booleans true or false
    [ClientRpc]
    void rpc_SetSteam(string parameterToSet, bool setting)
    {
        Debug.Log("Called RPC to set steam");
        animator.SetBool(parameterToSet, setting);
    }

    // Call an RPC to set objects to avtice or inactive
    [Command(requiresAuthority = false)]
    void cmd_objectEnable(string target, bool setting)
    {
        rpc_objectEnable(target, setting);
        //local_objectEnable(target, setting);
        Debug.Log("Called Command to set object");
    }

    // Set objects to avtice or inactive
    [ClientRpc]
    void rpc_objectEnable(string target, bool setting){
        Debug.Log("Called RPC to set object");
        local_objectEnable(target, setting);
    }    
}
