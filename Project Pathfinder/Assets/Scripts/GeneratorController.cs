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
    public GameObject breakPopup;
    public int healthPoints = 10;
    public float healthBarXScale;
    public GameObject healthBar;
    private bool repairingGenerator = false;

    private Player_UI playerUi;   // Player UI management script
    public float waitTime = 1f;   // Time to wait in between repairing individual generator hp
    public float nextHealTime;    // Next time to heal 1 generator hp

    // Property for runner
    public GameObject runner {
        get
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
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

    void Start(){
        // Assign Player UI script
        playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();

        // Assign health bar scale
        healthBarXScale = healthBar.transform.localScale.x;

        // Communicate to the CNM that at least ONE steam generator is spawned
        CustomNetworkManager.steamGeneratorsSpawned = true;
    }

    void Update(){
        // Controls turning on and off steam generators when you need to generate
        if(animator.GetBool("IsGenerating") == false && GenerateSteam.steam < 100f && animator.GetBool("IsBusted") == false){
            animator.SetBool("IsGenerating", true);
        }
        else if(animator.GetBool("IsGenerating") == true && GenerateSteam.steam >= 100f && animator.GetBool("IsBusted") == false){
            animator.SetBool("IsGenerating", false);
        }

        // Set health bar length
        healthBar.transform.localScale = new Vector3(healthBarXScale * (healthPoints / 10f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);

        // Set health bar color based on generator status
        if(!animator.GetBool("IsBusted")){
            healthBar.GetComponent<SpriteRenderer>().color = new Color(190f, 0f, 0f);
        }
        else{
            healthBar.GetComponent<SpriteRenderer>().color = new Color(126f, 126f, 126f);
        }

        // Detects if the Engineer is near to fix the generator
        if(animator.GetBool("IsBusted") == true && healthPoints < 10 && Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 0.8f, gameObject.transform.position.y -1.7f), engineer.transform.position) < 3f){
            
            // Set next repair time
            if(!repairingGenerator){
                repairingGenerator = true;
                nextHealTime += waitTime;
            }
            if(Time.time > nextHealTime){
                // Turn on Healing Touch effect
                if(engineer.transform.GetChild(2).gameObject.activeSelf == false){
                    GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_objectEnable("RepairingEffect", true, gameObject.name);
                }

                // Repair 1 generator hp
                healthPoints++;
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_SetGeneratorHealth(gameObject.name, healthPoints);

                // Turn generator back on and turn off Healing Touch effect
                if(healthPoints>= 10){
                    GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_objectEnable("RepairingEffect", false, gameObject.name);
                    GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_SetSteam("IsBusted", false, gameObject.name);
                }
            }
        }

        // Turn off repairing UI if away from Generator
        else if(animator.GetBool("IsBusted") == true && Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 0.8f, gameObject.transform.position.y -1.7f), engineer.transform.position) >= 3f){
            if(engineer.transform.GetChild(2).gameObject.activeSelf){
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_objectEnable("RepairingEffect", false, gameObject.name);
            }
            repairingGenerator = false;
        }

        // Reset repairing generator status
        else{
            repairingGenerator = false;
        }

        // Display the break popup
        if(CustomNetworkManager.isRunner){
            if(gameObject.GetComponent<Animator>().GetBool("IsBusted") == false && playerUi.activeSelectedItem == Item.ItemType.Sledge && Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 0.8f, gameObject.transform.position.y -1.7f), runner.transform.position) < 2.5f){
                breakPopup.SetActive(true);
                healthBar.SetActive(true);
            }
            else{
                breakPopup.SetActive(false);
                healthBar.SetActive(false);
            }
        }
        else{
            if(Utilities.GetDistanceBetweenObjects(new Vector2(gameObject.transform.position.x + 0.8f, gameObject.transform.position.y -1.7f), HandleLayers.activeCharacterLocation) < 3f){
                healthBar.SetActive(true);
            }
            else{
                healthBar.SetActive(false);
            }
        }
    }

    // Detect if the runner can break the generator
    public static bool breakGenerator(){
        bool generatorBroken = false;
        Debug.Log("GeneratorController: breakGenerator called");
        GameObject generator = GeneratorController.FindClosestGenerator("Runner");
        GeneratorController generatorController = generator.GetComponent<GeneratorController>();

        if(generator.GetComponent<Animator>().GetBool("IsBusted") == false && Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 0.8f, generator.transform.position.y -1.7f), generator.GetComponent<GeneratorController>().runner.transform.position) < 2.5f && generator.GetComponent<GeneratorController>().runner.GetComponent<MoveCharacter>().canMove == true)
        {
            if(generatorController.healthPoints > 1){
                generatorController.healthPoints--;
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_SetGeneratorHealth(generator.name, generatorController.healthPoints);
            }
            else{
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().cmd_SetSteam("IsBusted", true, generator.name);
                generatorBroken = true; 
                generatorController.healthPoints--;
            }
            
        }
        Debug.Log("GeneratorController breakGenerator(): Distance between runner and generator is:" + Utilities.GetDistanceBetweenObjects(new Vector2(generator.transform.position.x + 0.5f, generator.transform.position.y -1f), generator.GetComponent<GeneratorController>().runner.transform.position).ToString());
        
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

    public void local_objectEnable(string target, bool setting){
        Debug.Log("Called locally to set object");
        if(target == "RepairingEffect"){
            if(repairingEffect == null){
                Debug.LogWarning("REPAIRING EFFECT IS NULL");
            }
            else{
                repairingEffect.SetActive(setting);
            }
        }
    }
}
